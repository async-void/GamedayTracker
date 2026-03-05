using GamedayTracker.Interfaces;
using GamedayTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GamedayTracker.Helpers
{
    public sealed class JsonDmQueue : IDmQueue
    {
        private readonly string _filePath;
        private readonly object _lock = new();
        private readonly Queue<object> _queue = new();

        public JsonDmQueue(string filePath)
        {
            _filePath = filePath;
            Load();
        }

        public void Enqueue(object payload)
        {
            lock (_lock)
            {
                _queue.Enqueue((DmPayload)payload);
                Save();
            }
        }

        public bool TryDequeue(out object? payload)
        {
            lock (_lock)
            {
                if (_queue.Count == 0)
                {
                    payload = null;
                    return false;
                }

                payload = _queue.Dequeue() as DmPayload;
                Save();
                return true;
            }
        }

        private void Load()
        {
            if (!File.Exists(_filePath))
                return;

            try
            {
                var json = File.ReadAllText(_filePath);
                var items = JsonSerializer.Deserialize<List<DmPayload>>(json);

                if (items is not null)
                {
                    foreach (var item in items)
                        _queue.Enqueue(item);
                }
            }
            catch
            {
                // If the file is corrupted, start fresh.
            }
        }

        private void Save()
        {
            var items = _queue.ToList();
            var json = JsonSerializer.Serialize(items, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(_filePath, json);
        }


    }
}
