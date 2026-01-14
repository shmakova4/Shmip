using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Todo.Entities;

namespace Desktop.Repository
{
    public class TaskRepository
    {
        private const string TasksDirectory = @"C:\Users\User\Desktop\2 курс\Todo\Tasks\";
        private static readonly object _lock = new object();

        public TaskRepository()
        {
            lock (_lock)
            {
                if (!Directory.Exists(TasksDirectory))
                {
                    Directory.CreateDirectory(TasksDirectory);
                }
            }
        }

        public List<TaskModel> GetUserTasks(string username)
        {
            lock (_lock)
            {
                string userTasksFile = Path.Combine(TasksDirectory, $"{username}_tasks.json");

                if (File.Exists(userTasksFile))
                {
                    try
                    {
                        string json = File.ReadAllText(userTasksFile);
                        if (!string.IsNullOrEmpty(json))
                        {
                            var options = new JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true
                            };

                            var tasks = JsonSerializer.Deserialize<List<TaskModel>>(json, options);
                            return tasks ?? new List<TaskModel>();
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ошибка загрузки задач: {ex.Message}");
                    }
                }

                return new List<TaskModel>();
            }
        }

        public bool SaveUserTasks(string username, List<TaskModel> tasks)
        {
            lock (_lock)
            {
                try
                {
                    string userTasksFile = Path.Combine(TasksDirectory, $"{username}_tasks.json");
                    var options = new JsonSerializerOptions
                    {
                        WriteIndented = true
                    };

                    string json = JsonSerializer.Serialize(tasks, options);
                    File.WriteAllText(userTasksFile, json);
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка сохранения задач: {ex.Message}");
                    return false;
                }
            }
        }

        public bool UserHasTasks(string username)
        {
            lock (_lock)
            {
                var tasks = GetUserTasks(username);
                return tasks != null && tasks.Count > 0;
            }
        }

        public bool AddTask(TaskModel task)
        {
            lock (_lock)
            {
                try
                {
                    var tasks = GetUserTasks(task.Username);
                    tasks.Add(task);
                    return SaveUserTasks(task.Username, tasks);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка добавления задачи: {ex.Message}");
                    return false;
                }
            }
        }

        public bool DeleteTask(string username, string taskId)
        {
            lock (_lock)
            {
                try
                {
                    var tasks = GetUserTasks(username);
                    var taskToRemove = tasks.FirstOrDefault(t => t.Id == taskId);

                    if (taskToRemove != null)
                    {
                        tasks.Remove(taskToRemove);
                        return SaveUserTasks(username, tasks);
                    }

                    return false;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка удаления задачи: {ex.Message}");
                    return false;
                }
            }
        }

        public bool UpdateTask(TaskModel updatedTask)
        {
            lock (_lock)
            {
                try
                {
                    var tasks = GetUserTasks(updatedTask.Username);
                    var existingTask = tasks.FirstOrDefault(t => t.Id == updatedTask.Id);

                    if (existingTask != null)
                    {
                        tasks.Remove(existingTask);
                        tasks.Add(updatedTask);
                        return SaveUserTasks(updatedTask.Username, tasks);
                    }

                    return false;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка обновления задачи: {ex.Message}");
                    return false;
                }
            }
        }
    }
}