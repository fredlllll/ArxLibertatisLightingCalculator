﻿using BepuPhysics;
using BepuPhysics.Collidables;
using BepuPhysics.CollisionDetection;
using BepuUtilities;
using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace ArxLibertatisLightingCalculatorLib.Bepu
{
    public struct NoCollisionCallbacks : INarrowPhaseCallbacks
    {
        public void Initialize(Simulation simulation)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool AllowContactGeneration(int workerIndex, CollidableReference a, CollidableReference b)
        {
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool AllowContactGeneration(int workerIndex, CollidablePair pair, int childIndexA, int childIndexB)
        {
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ConfigureContactManifold<TManifold>(int workerIndex, CollidablePair pair, ref TManifold manifold, out PairMaterialProperties pairMaterial) where TManifold : struct, IContactManifold<TManifold>
        {
            pairMaterial = new PairMaterialProperties();
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ConfigureContactManifold(int workerIndex, CollidablePair pair, int childIndexA, int childIndexB, ref ConvexContactManifold manifold)
        {
            return false;
        }

        public void Dispose()
        {
        }
    }

    public struct DemoPoseIntegratorCallbacks : IPoseIntegratorCallbacks
    {
        /// <summary>
        /// Gravity to apply to dynamic bodies in the simulation.
        /// </summary>
        public Vector3 Gravity;
        /// <summary>
        /// Fraction of dynamic body linear velocity to remove per unit of time. Values range from 0 to 1. 0 is fully undamped, while values very close to 1 will remove most velocity.
        /// </summary>
        public float LinearDamping;
        /// <summary>
        /// Fraction of dynamic body angular velocity to remove per unit of time. Values range from 0 to 1. 0 is fully undamped, while values very close to 1 will remove most velocity.
        /// </summary>
        public float AngularDamping;

        Vector3 gravityDt;
        float linearDampingDt;
        float angularDampingDt;

        public AngularIntegrationMode AngularIntegrationMode => AngularIntegrationMode.Nonconserving;

        public void Initialize(Simulation simulation)
        {
            //In this demo, we don't need to initialize anything.
            //If you had a simulation with per body gravity stored in a CollidableProperty<T> or something similar, having the simulation provided in a callback can be helpful.
        }

        /// <summary>
        /// Creates a new set of simple callbacks for the demos.
        /// </summary>
        /// <param name="gravity">Gravity to apply to dynamic bodies in the simulation.</param>
        /// <param name="linearDamping">Fraction of dynamic body linear velocity to remove per unit of time. Values range from 0 to 1. 0 is fully undamped, while values very close to 1 will remove most velocity.</param>
        /// <param name="angularDamping">Fraction of dynamic body angular velocity to remove per unit of time. Values range from 0 to 1. 0 is fully undamped, while values very close to 1 will remove most velocity.</param>
        public DemoPoseIntegratorCallbacks(Vector3 gravity, float linearDamping = .03f, float angularDamping = .03f) : this()
        {
            Gravity = gravity;
            LinearDamping = linearDamping;
            AngularDamping = angularDamping;
        }

        public void PrepareForIntegration(float dt)
        {
            //No reason to recalculate gravity * dt for every body; just cache it ahead of time.
            gravityDt = Gravity * dt;
            //Since these callbacks don't use per-body damping values, we can precalculate everything.
            linearDampingDt = MathF.Pow(MathHelper.Clamp(1 - LinearDamping, 0, 1), dt);
            angularDampingDt = MathF.Pow(MathHelper.Clamp(1 - AngularDamping, 0, 1), dt);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void IntegrateVelocity(int bodyIndex, in RigidPose pose, in BodyInertia localInertia, int workerIndex, ref BodyVelocity velocity)
        {
            //Note that we avoid accelerating kinematics. Kinematics are any body with an inverse mass of zero (so a mass of ~infinity). No force can move them.
            if (localInertia.InverseMass > 0)
            {
                velocity.Linear = (velocity.Linear + gravityDt) * linearDampingDt;
                velocity.Angular *= angularDampingDt;
            }
            //Implementation sidenote: Why aren't kinematics all bundled together separately from dynamics to avoid this per-body condition?
            //Because kinematics can have a velocity- that is what distinguishes them from a static object. The solver must read velocities of all bodies involved in a constraint.
            //Under ideal conditions, those bodies will be near in memory to increase the chances of a cache hit. If kinematics are separately bundled, the the number of cache
            //misses necessarily increases. Slowing down the solver in order to speed up the pose integrator is a really, really bad trade, especially when the benefit is a few ALU ops.

            //Note that you CAN technically modify the pose in IntegrateVelocity by directly accessing it through the Simulation.Bodies.ActiveSet.Poses, it just requires a little care and isn't directly exposed.
            //If the PositionFirstTimestepper is being used, then the pose integrator has already integrated the pose.
            //If the PositionLastTimestepper or SubsteppingTimestepper are in use, the pose has not yet been integrated.
            //If your pose modification depends on the order of integration, you'll want to take this into account.

            //This is also a handy spot to implement things like position dependent gravity or per-body damping.
        }

    }
}
