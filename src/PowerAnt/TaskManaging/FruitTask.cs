using AntMe.English;

namespace AntMe.Player.PowerAnt.TaskManaging
{
    public class FruitTask : Task
    {
        public Fruit Fruit { get; private set; }

        public FruitTask(Fruit fruit)
        {
            Fruit = fruit;
        }
    }
}