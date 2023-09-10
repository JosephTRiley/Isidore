using Isidore.Render;

namespace Isidore_Tests
{
    /// <summary>
    /// Test that the render classes are behaving as expected
    /// </summary>
    class ClassTest
    {
        /// <summary>
        /// Runs class
        /// </summary>
        /// <returns> Success marker (0=fail, 1=succeed) </returns>
        public static bool Run()
        {
            // Checking Items/Projectors/Shapes class
            Sphere sphere1 = new Sphere();
            Sphere sphere2 = new Sphere();

            // Adds a new time line to the second shape
            // (If I reference Isidore.Math, then I have to specify
            // which sphere (Math or Render) to use
            Isidore.Maths.KeyFrameTrans oldTrans =
                sphere2.TransformTimeLine.Clone();
            Isidore.Maths.KeyFrameTrans newTrans = new 
                Isidore.Maths.KeyFrameTrans(
                Isidore.Maths.Transform.Translate(1, 1, 1));
            sphere2.TransformTimeLine = newTrans;

            // Adds he two shapes to a shape list
            Shapes shapes = new Shapes
            {
                sphere1,
                sphere2
            };

            // This should be a reference until shapes[0] is deleted
            Shape refSphere0 = shapes[0];

            // Checks memberwise clone
            shapes[1].IntersectBackFaces = false;
            Shape cpSphere1 = shapes[1].Clone();
            shapes[1].IntersectBackFaces = true;

            // Re-assigns the original key-frame transform
            sphere2.TransformTimeLine = oldTrans;

            // Removes the first instance
            shapes.RemoveAt(0);

            // Resets to the default ID, should increment
            refSphere0.ID = -1;
            shapes.Add(refSphere0);
            shapes.Add(cpSphere1);

            // Checks that ID is incremented
            for (int idx = 0; idx < shapes.Count; idx++)
                if (shapes[idx].ID != idx + 1)
                    return false;

            // Checks that back face values are maintained
            // (shape[1] is shape[0] after RemoveAt(0) 
            if (cpSphere1.IntersectBackFaces || !shapes[0].IntersectBackFaces)
                return false;

            // Checks the transformers to make sure cloning is working
            double[,] diff1 = Isidore.Maths.Operator.Subtract(
                sphere1.TransformTimeLine.Values[0].M,
                sphere2.TransformTimeLine.Values[0].M);
            double sumDiff1 = Isidore.Maths.Stats.Sum(diff1);
            double[,] diff2 = Isidore.Maths.Operator.Subtract(
                cpSphere1.TransformTimeLine.Values[0].M,
                sphere2.TransformTimeLine.Values[0].M);
            double sumDiff2 = Isidore.Maths.Stats.Sum(diff2);

            if (sumDiff1 != 0 || sumDiff2 != 3)
                return false;

            return true;
        }
    }
}
