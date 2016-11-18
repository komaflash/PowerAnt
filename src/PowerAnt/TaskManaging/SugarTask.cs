using AntMe.English;

namespace AntMe.Player.PowerAnt.TaskManaging
{

    public class SugarTask : Task
    {
        public Sugar Sugar { get; private set; }

        public SugarTask(Sugar sugar)
        {
            Sugar = sugar;
        }
    }

}