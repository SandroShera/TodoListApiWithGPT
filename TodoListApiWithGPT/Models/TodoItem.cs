namespace TodoListApiWithGPT.Models
{
    public class TodoItem
    {
        //Primary Key
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public bool IsCompleted { get; set; } = false;
    }
}
