using SC.Ecs.Anim2d;

public class SampleAnimatorAuthoring : BaseAnimatorAuthoring
{
    public class SampleAnimatorBaker : AnimatorBaker<SampleAnimatorAuthoring>
    {
        public override void Bake(SampleAnimatorAuthoring authoring)
        {
            /*
            Overriding bake logic
            ...
            */

            base.Bake(authoring);
        }
    }
}
