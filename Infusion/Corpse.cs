namespace Infusion
{
    public class Corpse : Item
    {
        public ModelId CorpseType => base.Amount;

        public Corpse(ObjectId id, ModelId type, ushort amount, Location3D location, Color? color, ObjectId? containerId, Layer? layer)
            : base(id, type, amount, location, color, containerId, layer)
        {
        }

        protected override GameObject Duplicate()
        {
            var duplicate = new Corpse(Id, Type, Amount, Location, Color, ContainerId, Layer);

            duplicate.CopyFrom(this);

            return duplicate;
        }

        public override string ToString() 
            => $"Id: {Id}; Type: {Type}; CorpseType: {CorpseType}; Name: {Name}; Location: {Location}; Color: {Color}; ";
    }
}