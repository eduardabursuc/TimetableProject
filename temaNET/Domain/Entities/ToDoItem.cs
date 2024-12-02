// Domain/Entities/ToDoItem.cs
using System;

namespace Domain.Entities
{
    public class ToDoItem
    {
        public Guid Id { get; private set; }
        public string Description { get; set; }

        private bool _isDone;
        public bool IsDone
        {
            get => _isDone;
            set
            {
                if (!_isDone && value)
                {
                    _isDone = true;
                    EndDate = DateTime.Now;
                }
                else
                {
                    _isDone = value;
                }
            }
        }

        public DateTime StartDate { get; private set; }
        public DateTime? EndDate { get; private set; }

        public ToDoItem()
        {
            Id = Guid.NewGuid();
            StartDate = DateTime.Now;
        }
    }
}