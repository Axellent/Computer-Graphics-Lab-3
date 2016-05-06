using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameEngine.InputDefs;
using GameEngine;
using Microsoft.Xna.Framework;

namespace NAJ_Lab3 {
    class ChopperControlSystem : IUpdateSystem {
        ECSEngine engine;

        public ChopperControlSystem(ECSEngine engine) {
            this.engine = engine;
        }
        public void Update(GameTime gameTime) {
            List<Entity> sceneEntities = SceneManager.Instance.GetActiveScene().GetAllEntities();
            Entity chopper = ComponentManager.Instance.GetEntityWithTag("Chopper", sceneEntities);
            TransformComponent t = ComponentManager.Instance.GetEntityComponent<TransformComponent>(chopper);
            ModelComponent chopModel = ComponentManager.Instance.GetEntityComponent<ModelComponent>(chopper);

            Entity modelCounter = ComponentManager.Instance.GetFirstEntityOfType<ModelCountComponent>();
            ModelCountComponent modelCount = ComponentManager.Instance.GetEntityComponent<ModelCountComponent>(modelCounter);

            Entity terrain = ComponentManager.Instance.GetEntityWithTag("Terrain", sceneEntities);
            TerrainMapComponent tcomp = ComponentManager.Instance.GetEntityComponent<TerrainMapComponent>(terrain);

            engine.SetWindowTitle("Num Chunks in camera frustrum:" + tcomp.numChunksInView + "    Num Models in camera frustrum:" + modelCount.numModelsInView);

            //lock the model to groundheight
            //t.position = new Vector3(t.position.X, 1.7f + TerrainMapRenderSystem.GetTerrainHeight(tcomp, t.position.X, Math.Abs(t.position.Z)), t.position.Z);

            //set the mesh transforms to zero
            ModelRenderSystem.SetMeshTransform(ref chopModel, 1, Matrix.CreateRotationY(0.0f));
            ModelRenderSystem.SetMeshTransform(ref chopModel, 3, Matrix.CreateRotationY(0.0f));

            Entity kb = ComponentManager.Instance.GetEntityWithTag("keyboard", sceneEntities);
            if (kb != null) {
                KeyBoardComponent k = ComponentManager.Instance.GetEntityComponent<KeyBoardComponent>(kb);
                if (k != null) {
                    Vector3 newRot = Vector3.Zero;
                    bool moving = false;

                    if (Utilities.CheckKeyboardAction("right", BUTTON_STATE.HELD, k)) {
                        newRot = new Vector3(-2.8f, 0f, 0f) * (float)gameTime.ElapsedGameTime.TotalSeconds;
                        t.vRotation = newRot;
                        moving = true;
                    }
                    else if (Utilities.CheckKeyboardAction("left", BUTTON_STATE.HELD, k)) {
                        newRot = new Vector3(2.8f, 0f, 0f) * (float)gameTime.ElapsedGameTime.TotalSeconds;
                        t.vRotation = newRot;
                        moving = true;
                    }
                    else {
                        t.vRotation = Vector3.Zero;
                    }

                    if (Utilities.CheckKeyboardAction("up", BUTTON_STATE.HELD, k)) {
                        t.position += new Vector3(0f, 70f, 0f) * (float)gameTime.ElapsedGameTime.TotalSeconds;
                        moving = true;
                    }

                    if (Utilities.CheckKeyboardAction("quit", BUTTON_STATE.RELEASED, k)) {
                        System.Environment.Exit(0);
                    }
                    if (Utilities.CheckKeyboardAction("down", BUTTON_STATE.HELD, k)) {
                        t.position += new Vector3(0f, -70f, 0f) * (float)gameTime.ElapsedGameTime.TotalSeconds;
                        moving = true;
                    }
                    if (Utilities.CheckKeyboardAction("forward", BUTTON_STATE.HELD, k)) {
                        t.position += t.forward * 100f * (float)gameTime.ElapsedGameTime.TotalSeconds;
                        moving = true;
                    }
                    if (Utilities.CheckKeyboardAction("back", BUTTON_STATE.HELD, k)) {
                        t.position += t.forward * -100f * (float)gameTime.ElapsedGameTime.TotalSeconds;
                        moving = true;
                    }

                    if (moving == true) {
                        ModelRenderSystem.SetMeshTransform(ref chopModel, 1, Matrix.CreateRotationY(0.08f));
                        ModelRenderSystem.SetMeshTransform(ref chopModel, 3, Matrix.CreateRotationY(0.1f));
                    }
                }
            }
        }
    }
}
