namespace Isidore.Render
{
    /// <summary>
    /// Scene is used to layout out the scenario to be rendered.
    /// </summary>
    public class Scene
    {
        #region Fields & Properties      

        /// <summary>
        /// The scene's current simulation time [sec]
        /// </summary>
        public double CurrentTime { get { return currentTime; } }
        private double currentTime = double.NaN; // Current scene time

        /// <summary>
        /// The scene's projectors. I.e. cameras, lasers, etc.
        /// </summary>
        public Projectors Projectors;

        /// <summary>
        /// The scene's bodies.  A body is any
        /// physical boundary object that affects a ray.
        /// </summary>
        public Bodies Bodies;

        /// <summary>
        /// The Scene's light sources.  These are indirect light sources such as point sources.
        /// </summary>
        public Lights Lights;

        /// <summary>
        /// Signals whether to use multiple cores
        /// </summary>
        public bool UseMultiCores { get; set; } = true;


        #endregion Fields & Properties
        #region Constructors

        /// <summary>
        /// Scene default constructor.
        /// </summary>
        public Scene()
        {
            Projectors = new Projectors();
            Bodies = new Bodies();
            Lights = new Lights();
        }

        #endregion Constructors
        #region Methods

        /// <summary>
        /// Advances the scene time to "now" by advancing every item, then
        /// ray-tracing through the projectors in order of array position
        /// </summary>
        /// <param name="now"> Current simulation time </param>
        /// <param name="force"> Forces AdvanceToTime to run even if the time is the same </param>
        public void AdvanceToTime(double now, bool force = false)
        {
            if (!force && now == currentTime)
                return;

            // Sets current time to now
            currentTime = now;

            // Sets all shapes to now
            //Bodies.ForEach(body => body.AdvanceToTime(now, force));
            // Used a for loop so I can step into the loop
            for (int idx = 0; idx < Bodies.Count; idx++)
                Bodies[idx].AdvanceToTime(now, force);

            // Sets all projectors to now
            //Projectors.ForEach(proj => proj.AdvanceToTime(now, force));
            // Used a for loop so I can step into the loop
            for (int idx = 0; idx < Projectors.Count; idx++)
                Projectors[idx].AdvanceToTime(now, force);

            // Checks if there are any bodies present, if not, skips the trace
            if (Bodies.Count == 0)
                return;

            // Applies ray-tracing for each projection with every object
            for (int idx = 0; idx < Projectors.Count; idx++)
            {
                // Projector instance
                Projector proj = Projectors[idx];
                // Continues until all raytrees are closed
                while (proj.AnyOpenRayTrees())
                    Render(ref proj);
            }
        }
        /// <summary>
        /// Produces intersection data tree for the projector
        /// </summary>
        /// <param name="proj"> Projector instance </param>
        /// <returns> Intersection data tree array </returns>
        public void Render(ref Projector proj)
        {
            // Accesses each surface's intersection application
            // Lambda operations aren't permitted with references
            for (int idx = 0; idx < Bodies.Count; idx++)
                if(UseMultiCores)
                    Bodies[idx].MultiCoreIntersect(ref proj);
                else
                    Bodies[idx].OneCoreIntersect(ref proj);


            // Updates the ray tree associated with each pixel
            proj.UpdateRayTrees();

            // Shader
            //if (scene.Lights.Count > 0)
            //    Shade(scene, ref iData);
        }

        #endregion Methods
    }
}
