using AntMe.English;

namespace AntMe.Player.PowerAnt.TaskManaging
{
    public class FoodTask : Task
    {
        public Food Target { get; private set; }

        public FoodTask(Food food)
        {
            Target = food;
        }
    }
}