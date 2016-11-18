using AntMe.English;

namespace AntMe.Player.PowerAnt
{
    /// <summary>
    /// Contains all collector specific logic
    /// </summary>
    public partial class PowerAntClass
    {
        void CollectorSpotsFruit(Fruit fruit)
        {
            if (CustomTarget == fruit || Destination == fruit)
            {
                GoToDestination(fruit);
                task = null;
            }
        }

        void CollectorSpotsSugar(Sugar sugar)
        {           
            if (CustomTarget == sugar || Destination == sugar)
            {
                GoToDestination(sugar);
                task = null;
            }
        }

        void CollectorDestinationReached(Food food)
        {
            if (food is Fruit)
            {
                var x = (Fruit)food;
                if (NeedsCarrier(x))
                {
                    Take(x);
                    GoTo(Anthill);
                }
            }
            else if (food is Sugar)
            {
                var x = (Sugar)food;
                if (x.Amount > 0)
                {
                    Take(x);
                    GoTo(Anthill);
                }
            }
        }
    }
}
