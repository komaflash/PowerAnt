using AntMe.English;
using AntMe.Player.PowerAnt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AntMe.Player.PowerAnt
{
    public class Task
    {
    }

    public class SugarTask : Task
    {
        public Sugar Sugar { get; private set; }

        public SugarTask(Sugar sugar)
        {
            Sugar = sugar;
        }
    }

    public class FruitTask : Task
    {
        public Fruit Fruit { get; private set; }

        public FruitTask(Fruit fruit)
        {
            Fruit = fruit;
        }
    }

    internal class TaskManager
    {
        Logger log = new Logger();

        #region singleton impl
        private static TaskManager _instance;

        public static TaskManager Instance
        {
            get
            {
                if (_instance == null) _instance = new TaskManager();
                return _instance;
            }
        }

        #endregion

        private Queue<Task> collectorQ = new Queue<Task>();
        private Queue<Task> hunterQ = new Queue<Task>();

        public List<PowerAntClass> Worker { get; private set; } = new List<PowerAntClass>();
        public List<Sugar> Sugars { get; private set; } = new List<Sugar>();
        public List<Fruit> Fruits { get; private set; } = new List<Fruit>();

        public void RegisterWorker(PowerAntClass ant)
        {
            if (!Worker.Contains(ant))
            {
                Worker.Add(ant);
                log.Debug($"Worker registered {ant.Id}");
            }
        }

        public void UnregisterWorker(PowerAntClass ant, Task task = null)
        {
            if (Worker.Contains(ant))
            {
                if (task != null)
                {
                    // if (task is SugarTask)
                    collectorQ.Enqueue(task);
                    //if (task is FruitTask)
                    //   fruitQ.Enqueue(task);
                }

                Worker.Remove(ant);
                log.Debug($"Worker unregistered {ant.Id}");
            }
        }

        public void ReportSugar(Sugar sugar)
        {
            int existingTickets = 0;

            if (Sugars.Contains(sugar))
            {
                existingTickets = collectorQ.Count(x => x is SugarTask && ((SugarTask)x).Sugar == sugar);
                if (existingTickets > 0)
                {
                    return;
                }

                // we know the sugar, but there are no tasks anymore for it
                // this could be related to killed ants on their way back home
            }
            else
            {
                Sugars.Add(sugar);
            }

            var taskCount = (sugar.Amount - (existingTickets * 10)) / 10; //todo: get max load dynamicly
            for (int i = 0; i < taskCount; i++)
            {
                collectorQ.Enqueue(new SugarTask(sugar));
            }

            log.Debug($"New sugar reported, enqueued {taskCount} tasks.");
        }

        public void ReportFruit(Fruit fruit)
        {
            if (Fruits.Contains(fruit)) return;

            Fruits.Add(fruit);
            var taskCount = 3;
            for (int i = 0; i < taskCount; i++)
            {
                collectorQ.Enqueue(new FruitTask(fruit));
            }

            log.Debug($"New fruit reported, enqueued {taskCount} tasks.");
        }

        public Task GetTask(string caste)
        {
            if (caste == Consts.Collector && collectorQ.Any())
            {
                return collectorQ.Dequeue();
            }
            else if (caste == Consts.Hunter && hunterQ.Any())
            {
                return hunterQ.Dequeue();
            }

            return null;
        }
    }
}
