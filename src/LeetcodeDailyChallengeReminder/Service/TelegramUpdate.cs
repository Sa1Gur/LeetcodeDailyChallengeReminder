public record TelegramUpdate
{
    public bool Ok { get; set; }
    public required List<MessageUpdate> Result { get; set; }

    public record MessageUpdate
    {
        public int UpdateId { get; set; }
        public required Message Message { get; set; }
    }

    public record Message
    {
        public int MessageId { get; set; }
        public required From From { get; set; }
        public required Chat Chat { get; set; }
        public int Date { get; set; }
        public required string Text { get; set; }
    }

    public record From
    {
        public required string Username { get; set; }
    }

    public record Chat
    {
        public long Id { get; set; }
    }
}