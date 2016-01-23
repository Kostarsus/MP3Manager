using Hqub.MusicBrainz.API;
using Hqub.MusicBrainz.API.Entities;
using MP3ManagerBase.model;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace MP3ManagerBase.manager
{
    public class AlbumAdditionalReader
    {
        #region Inner Classes
        internal class AlbumInformation
        {
            /// <summary>
            /// Der Name des Interpreten
            /// </summary>
            public string ArtistName { get; set; }

            /// <summary>
            /// Der Name desAlbums
            /// </summary>
            public string AlbumName { get; set; }

            /// <summary>
            /// Die ID unter welcher der Interpret gespeichert wurde
            /// </summary>
            public string ArtistMBId { get; set; }

            /// <summary>
            /// Die ID unter welchre das Album gespiechert ist
            /// </summary>
            public string AlbumMBId { get; set; }

            /// <summary>
            /// Der Name des Titels 
            /// </summary>
            public string TitleName { get; set; }

            /// <summary>
            /// DieID unterwelcher der Titel gespiechert ist
            /// </summary>
            public string TitleMBId { get; set; }
        }

        #endregion

        #region Fields
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region Methoden
        /// <summary>
        /// Aktualisiert die Albuminformationen aus MusicBrainz
        /// </summary>
        /// <returns></returns>
        public async Task UpdateAlbumInformation()
        {
            var titlesToUpload = MP3DataMgr.Instance.ReadTitlesToDownloadInformation();

            foreach (var item in titlesToUpload)
            {
                var albumInformation = await ReadTitlesFor(item.ArtistName, item.AlbumName);

                List<MusicBrainzInformation> newItems = new List<MusicBrainzInformation>();
                foreach (var title in albumInformation)
                {
                    var newInformation = new MusicBrainzInformation()
                    {
                        AlbumId = item.AlbumId,
                        AlbumMBId = title.AlbumMBId,
                        ArtistId = item.ArtistId,
                        ArtistMBId = title.ArtistMBId,
                        Title = title.TitleName,
                        TitleMBId = title.TitleMBId
                    };
                    newItems.Add(newInformation);
                }
                MP3DataMgr.Instance.AddMusicBrainzEntities(item.AlbumId, newItems);

                // Warte 1 Sekunde vor die nächste Verarbeitung. Hiermit wird sichergestellt, dass der MusicBrainz-Server nicht überlastet und die IP gesperrt wird.
                Task.Delay(1000).Wait();
            }
        }

        /// <summary>
        /// Diese Methode liest die Informationen zum angegebenen Album 
        /// </summary>
        /// <param name="artistName">
        /// Der Namedes Interpreten
        /// </param>
        /// <param name="albumName">
        /// Der Name des Albums
        /// </param>
        /// <returns>
        /// Die registrierten Titel zum Album
        /// </returns>
        internal async Task<List<AlbumInformation>> ReadTitlesFor(string artistName, string albumName)
        {
            List<AlbumInformation> result = new List<AlbumInformation>();

            var artist = (await Artist.SearchAsync(artistName)).First();

            string query = string.Format("aid=({0}) release=({1})", artist.Id, albumName);
            log.Debug(string.Format("Album-Query: {0}", query));

            var album = (await Release.SearchAsync(Uri.EscapeUriString(query), 10)).First();
            var release = await Release.GetAsync(album.Id, "recordings");

            foreach (var medium in release.MediumList)
            {
                foreach (var track in medium.Tracks)
                {
                    var recording = track.Recordring;
                    log.Debug(string.Format("{0}, {1}: {2}", artistName, albumName, recording.Title));
                    AlbumInformation newItem = new AlbumInformation()
                    {
                        AlbumName = albumName,
                        AlbumMBId = album.Id,
                        ArtistName = artistName,
                        ArtistMBId = artist.Id,
                        TitleName = recording.Title,
                        TitleMBId = recording.Id
                    };
                    result.Add(newItem);
                }
            }

            return result;
        }
        #endregion
    }
}
