namespace MP3ManagerBase.factory
{
    /// <summary>
    /// Dieses Enum stellt ein Flag dar, 
    /// in dem der Status der MP3-Tagsder Datei enthalten ist.
    /// Der eigentliche Status ist die Summe aller Status.
    /// Beispiel: Wenn Interpret und Album im MP3-Tag fehlt, wäre der Status 0x03.
    /// </summary>
    public enum ETagStatus
    {
        /// <summary>
        /// MP3-Tag konnte komplett gelesen werden
        /// </summary>
        TagStatusOk = 0x00,
        /// <summary>
        /// Der Interpret fehlte im MP3-Tag
        /// </summary>
        MissingInterpret = 0x01,
        /// <summary>
        /// Das Album fehlte im MP3-Tag
        /// </summary>
        MissingAlbum = 0x02,
        /// <summary>
        /// Der Titel fehlte im MP3-Tag
        /// </summary>
        MissingTitle = 0x04
    }
}