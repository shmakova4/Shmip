using System;

namespace Todo.Entities
{
    public class TaskModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public string Time { get; set; } 
        public bool IsCompleted { get; set; }
        public string Username { get; set; }
        public DateTime CreatedAt { get; set; }

        public TaskModel()
        {
            Id = Guid.NewGuid().ToString();
            Title = string.Empty;
            Category = "Дом";
            Description = string.Empty;
            Date = DateTime.Today;
            Time = "09:00";
            IsCompleted = false;
            Username = string.Empty;
            CreatedAt = DateTime.Now;
        }
    }
}