using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infusion.Packets
{
    public sealed class Mobile : GameObject
    {
        protected override GameObject Duplicate()
        {
            return new Mobile(Id, Type, Location)
            {
                Color = Color,
                CurrentHealth = CurrentHealth,
                MaxHealth = MaxHealth,
                Orientation = Orientation,
                Notoriety = Notoriety
            };
        }

        public Color? Color { get; private set; }
        public Movement? Orientation { get; private set; }
        public Notoriety? Notoriety { get; private set; }

        public Mobile(uint id, ModelId type, Location3D location, Color? color,
            Movement? orientation, Notoriety? notoriety)
            : base(id, type, location)
        {
            Color = color;
            Orientation = orientation;
            Notoriety = notoriety;
        }

        public override bool IsOnGround => true;

        public Mobile(uint id, ModelId type, Location3D location) : base(id, type, location)
        {            
        }

        public ushort CurrentHealth { get; private set; }

        public ushort MaxHealth { get; private set; }

        internal Mobile UpdateHealth(ushort currentHealth, ushort maxHealth)
        {
            var updatedItem = (Mobile)Duplicate();
            updatedItem.CurrentHealth = currentHealth;
            updatedItem.MaxHealth = maxHealth;

            return updatedItem;
        }

        public Mobile Update(ModelId type, Location3D location, Color color, Movement direction, Notoriety? notoriety)
        {
            var updatedMobile = (Mobile)Duplicate();
            updatedMobile.Location = location;
            updatedMobile.Type = type;
            updatedMobile.Color = color;
            updatedMobile.Orientation = direction;
            updatedMobile.Notoriety = notoriety;

            return updatedMobile;
        }


        public override string ToString()
        {
            return
                $"Id: {Id:X8}; Type: {Type}; Name: {Name}; Location: {Location}; Color: {Color}; Orientation: {Orientation}; Notiriety: {Notoriety}";
        }

    }
}
