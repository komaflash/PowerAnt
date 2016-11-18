using AntMe.English;
using AntMe.Player.PowerAnt.TaskManaging;
using AntMe.Player.PowerAnt.Utility;
using System;
using System.Collections.Generic;

namespace AntMe.Player.PowerAnt
{
    [Player(
        ColonyName = "PowerAnt",
        FirstName = "komaflash@github")]
    [Caste(
        Name = Consts.Collector,
        AttackModifier = -1,
        EnergyModifier = 0,
        LoadModifier = 2,
        RangeModifier = 0,
        RotationSpeedModifier = -1,
        SpeedModifier = 0,
        ViewRangeModifier = 0
    )]
    [Caste(
        Name = Consts.Scout,
        AttackModifier = -1,
        EnergyModifier = -1,
        LoadModifier = -1,
        RangeModifier = 1,
        RotationSpeedModifier = 0,
        SpeedModifier = 0,
        ViewRangeModifier = 2
    )]    
    public partial class PowerAntClass : BaseAnt
    {
        Anthill Anthill;
        string id;
        Task task;
        Item CustomTarget;

        public string Id { get { return id; } }

        #region Caste

        /// <summary>
        /// Every time that a new ant is born, its job group must be set. You can 
        /// do so with the help of the value returned by this method.
        /// Read more: "http://wiki.antme.net/en/API1:ChooseCaste"
        /// </summary>
        /// <param name="typeCount">Number of ants for every caste</param>
        /// <returns>Caste-Name for the next ant</returns>
        public override string ChooseCaste(Dictionary<string, int> typeCount)
        {
            id = Guid.NewGuid().ToString().Split('-')[0];

            if (typeCount[Consts.Collector] <= 95)
            {
                TaskManager.Instance.RegisterWorker(this);
                return Consts.Collector;
            }           

            return Consts.Scout;
        }

        #endregion

        #region Movement

        /// <summary>
        /// If the ant has no assigned tasks, it waits for new tasks. This method 
        /// is called to inform you that it is waiting.
        /// Read more: "http://wiki.antme.net/en/API1:Waiting"
        /// </summary>
        public override void Waiting()
        {
            if (Anthill == null)
            {
                GoToAnthill(); // Sets Destination to anthill
                Anthill = Destination as Anthill;
                Stop(); // stop to reset Destination                
            }

            if (Caste == Consts.Collector)
            {
                task = TaskManager.Instance.GetTask(this.Caste);
                if (task != null)
                {
                    if (task is FruitTask)
                        GoTo(((FruitTask)task).Fruit);
                    else if (task is SugarTask)
                        GoTo(((SugarTask)task).Sugar);
                }
                else
                {
                    TurnByDegrees(RandomNumber.Number(-30, 30));
                    GoForward(100);
                }
            }
            else if (Caste == Consts.Scout)
            {
                TurnByDegrees(RandomNumber.Number(-40, 40));
                GoForward(100);
            }            
        }

        /// <summary>
        /// This method is called when an ant has travelled one third of its 
        /// movement range.
        /// Read more: "http://wiki.antme.net/en/API1:GettingTired"
        /// </summary>
        public override void GettingTired()
        {
        }

        /// <summary>
        /// This method is called if an ant dies. It informs you that the ant has 
        /// died. The ant cannot undertake any more actions from that point forward.
        /// Read more: "http://wiki.antme.net/en/API1:HasDied"
        /// </summary>
        /// <param name="kindOfDeath">Kind of Death</param>
        public override void HasDied(KindOfDeath kindOfDeath)
        {
            TaskManager.Instance.UnregisterWorker(this, task);
        }

        /// <summary>
        /// This method is called in every simulation round, regardless of additional 
        /// conditions. It is ideal for actions that must be executed but that are not 
        /// addressed by other methods.
        /// Read more: "http://wiki.antme.net/en/API1:Tick"
        /// </summary>
        public override void Tick()
        {
            if (Anthill != null && CustomTarget == Anthill)
            {
                if (Coordinate.GetDistanceBetween(this, Anthill) < this.Viewrange)
                {
                    GoToDestination(Anthill);
                    CustomTarget = null;
                }
            }
        }

        #endregion

        #region Food

        /// <summary>
        /// This method is called as soon as an ant sees an apple within its 360° 
        /// visual range. The parameter is the piece of fruit that the ant has spotted.
        /// Read more: "http://wiki.antme.net/en/API1:Spots(Fruit)"
        /// </summary>
        /// <param name="fruit">spotted fruit</param>
        public override void Spots(Fruit fruit)
        {
            if (NeedsCarrier(fruit))
            {
                TaskManager.Instance.ReportFruit(fruit);
            }

            if (Caste == Consts.Collector)
                CollectorSpotsFruit(fruit);
        }

        /// <summary>
        /// This method is called as soon as an ant sees a mound of sugar in its 360° 
        /// visual range. The parameter is the mound of sugar that the ant has spotted.
        /// Read more: "http://wiki.antme.net/en/API1:Spots(Sugar)"
        /// </summary>
        /// <param name="sugar">spotted sugar</param>
        public override void Spots(Sugar sugar)
        {
            if (sugar.Amount > 0)
            {
                TaskManager.Instance.ReportSugar(sugar);
            }

            if (Caste == Consts.Collector)
                CollectorSpotsSugar(sugar);
        }

        /// <summary>
        /// If the ant’s destination is a piece of fruit, this method is called as soon 
        /// as the ant reaches its destination. It means that the ant is now near enough 
        /// to its destination/target to interact with it.
        /// Read more: "http://wiki.antme.net/en/API1:DestinationReached(Fruit)"
        /// </summary>
        /// <param name="fruit">reached fruit</param>
        public override void DestinationReached(Fruit fruit)
        {
            if (Caste == Consts.Collector)
                CollectorDestinationReached(fruit);
        }

        /// <summary>
        /// If the ant’s destination is a mound of sugar, this method is called as soon 
        /// as the ant has reached its destination. It means that the ant is now near 
        /// enough to its destination/target to interact with it.
        /// Read more: "http://wiki.antme.net/en/API1:DestinationReached(Sugar)"
        /// </summary>
        /// <param name="sugar">reached sugar</param>
        public override void DestinationReached(Sugar sugar)
        {
            if (Caste == Consts.Collector)
                CollectorDestinationReached(sugar);
        }

        #endregion

        #region Communication

        /// <summary>
        /// Friendly ants can detect markers left by other ants. This method is called 
        /// when an ant smells a friendly marker for the first time.
        /// Read more: "http://wiki.antme.net/en/API1:DetectedScentFriend(Marker)"
        /// </summary>
        /// <param name="marker">marker</param>
        public override void DetectedScentFriend(Marker marker)
        {
        }

        /// <summary>
        /// Just as ants can see various types of food, they can also visually detect 
        /// other game elements. This method is called if the ant sees an ant from the 
        /// same colony.
        /// Read more: "http://wiki.antme.net/en/API1:SpotsFriend(Ant)"
        /// </summary>
        /// <param name="ant">spotted ant</param>
        public override void SpotsFriend(Ant ant)
        {
        }

        /// <summary>
        /// Just as ants can see various types of food, they can also visually detect 
        /// other game elements. This method is called if the ant detects an ant from a 
        /// friendly colony (an ant on the same team).
        /// Read more: "http://wiki.antme.net/en/API1:SpotsTeammate(Ant)"
        /// </summary>
        /// <param name="ant">spotted ant</param>
        public override void SpotsTeammate(Ant ant)
        {
        }

        #endregion

        #region Fight

        /// <summary>
        /// Just as ants can see various types of food, they can also visually detect 
        /// other game elements. This method is called if the ant detects an ant from an 
        /// enemy colony.
        /// Read more: "http://wiki.antme.net/en/API1:SpotsEnemy(Ant)"
        /// </summary>
        /// <param name="ant">spotted ant</param>
        public override void SpotsEnemy(Ant ant)
        {
        }

        /// <summary>
        /// Just as ants can see various types of food, they can also visually detect 
        /// other game elements. This method is called if the ant sees a bug.
        /// Read more: "http://wiki.antme.net/en/API1:SpotsEnemy(Bug)"
        /// </summary>
        /// <param name="bug">spotted bug</param>
        public override void SpotsEnemy(Bug bug)
        {
        }

        /// <summary>
        /// Enemy creatures may actively attack the ant. This method is called if an 
        /// enemy ant attacks; the ant can then decide how to react.
        /// Read more: "http://wiki.antme.net/en/API1:UnderAttack(Ant)"
        /// </summary>
        /// <param name="ant">attacking ant</param>
        public override void UnderAttack(Ant ant)
        {
        }

        /// <summary>
        /// Enemy creatures may actively attack the ant. This method is called if a 
        /// bug attacks; the ant can decide how to react.
        /// Read more: "http://wiki.antme.net/en/API1:UnderAttack(Bug)"
        /// </summary>
        /// <param name="bug">attacking bug</param>
        public override void UnderAttack(Bug bug)
        {
        }

        #endregion

        #region custom

        // <summary>
        /// Go straight to <paramref name="destination"/>
        /// </summary>
        /// <param name="destination">destination</param>
        public void GoTo(Item destination)
        {
            var distance = Coordinate.GetDistanceBetween(this, destination);
            var angle = Coordinate.GetDegreesBetween(this, destination);
            TurnToDirection(angle);
            GoForward(distance);
            CustomTarget = destination;
        }

        #endregion custom

    }
}