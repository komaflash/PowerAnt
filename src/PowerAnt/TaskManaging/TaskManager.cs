using AntMe.English;
using AntMe.Player.PowerAnt.Utility;
using System.Collections.Generic;
using System.Linq;

namespace AntMe.Player.PowerAnt.TaskManaging
{
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

        /// <summary>
        /// Register an ant as worker in the task manager        
        /// </summary>
        /// <param name="ant"></param>
        public void RegisterWorker(PowerAntClass ant)
        {
            if (!Worker.Contains(ant))
            {
                Worker.Add(ant);
                log.Debug($"Worker registered {ant.Id}");
            }
        }

        /// <summary>
        /// Unregister a (e.g. dead) worker and make 
        /// sure that his remaining work is enqueued
        /// </summary>
        /// <param name="ant"></param>
        /// <param name="task"></param>
        public void UnregisterWorker(PowerAntClass ant, Task task = null)
        {
            if (Worker.Contains(ant))
            {
                if (task != null) { collectorQ.Enqueue(task); }
                Worker.Remove(ant);

                log.Debug($"Worker unregistered {ant.Id}, job={task}");
            }
        }

        /// <summary>
        /// Tell the task manager, that sugar was found
        /// </summary>
        /// <param name="sugar"></param>
        public void ReportSugar(Sugar sugar)
        {
            int existingTickets = 0;

            if (Sugars.Contains(sugar))
            {
                existingTickets = collectorQ.Count(x => x is FoodTask && ((FoodTask)x).Target == sugar);
                if (existingTickets > 0)
                {
                    return;
                }

                // we know the sugar, but there are no tasks anymore for it
                // this could be related to killed ants on their way back home
                // so we will create new ones.
            }
            else
            {
                Sugars.Add(sugar);
            }

            var taskCount = (sugar.Amount - (existingTickets * 10)) / 10; // TODO: get max load more dynamicly
            for (int i = 0; i < taskCount; i++)
            {
                collectorQ.Enqueue(new FoodTask(sugar));
            }

            log.Debug($"New sugar reported, enqueued {taskCount} tasks.");
        }

        /// <summary>
        /// Tell the task manager, that a fruit was found
        /// </summary>
        /// <param name="fruit"></param>
        public void ReportFruit(Fruit fruit)
        {
            if (Fruits.Contains(fruit)) return;

            Fruits.Add(fruit);
            var taskCount = 3;
            for (int i = 0; i < taskCount; i++)
            {
                collectorQ.Enqueue(new FoodTask(fruit));
            }

            log.Debug($"New fruit reported, enqueued {taskCount} tasks.");
        }

        /// <summary>
        /// Get a task to do some work
        /// </summary>
        /// <param name="caste"></param>
        /// <returns>null, if the queue is empty</returns>
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
