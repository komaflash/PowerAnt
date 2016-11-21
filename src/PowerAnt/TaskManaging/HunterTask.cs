using AntMe.English;

namespace AntMe.Player.PowerAnt.TaskManaging
{
    public class InsectTask : Task
    {
        public Insect Target { get; set; }

        public InsectTask(Insect insect)
        {
            Target = insect;
        }
    }
}
