using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GameEngine;

namespace NAJ_Lab3 {
    class InitGame {
        private SystemManager sm = SystemManager.Instance;

        public InitGame(ECSEngine engine) {
            InitKeyboard();
            InitChopper(engine);
            InitCamera(engine);
            InitTerrain(engine);

            sm.RegisterSystem("Game", new TransformSystem());
            sm.RegisterSystem("Game", new ModelRenderSystem());

            SceneManager.Instance.SetActiveScene("Game");
            SystemManager.Instance.Category = "Game";
        }

        private void InitKeyboard() {
            sm.RegisterSystem("Game", new KeyBoardSystem());

            Entity keyboardControl = EntityFactory.Instance.NewEntityWithTag("keyboard");
            ComponentManager.Instance.AddComponentToEntity(keyboardControl, new KeyBoardComponent());
            KeyBoardComponent k = ComponentManager.Instance.GetEntityComponent<KeyBoardComponent>(keyboardControl);

            KeyBoardSystem.AddKeyToAction(ref k, "forward", Keys.Up);
            KeyBoardSystem.AddKeyToAction(ref k, "back", Keys.Down);
            KeyBoardSystem.AddKeyToAction(ref k, "left", Keys.Left);
            KeyBoardSystem.AddKeyToAction(ref k, "right", Keys.Right);
            KeyBoardSystem.AddKeyToAction(ref k, "down", Keys.X);
            KeyBoardSystem.AddKeyToAction(ref k, "up", Keys.C);
            KeyBoardSystem.AddKeyToAction(ref k, "quit", Keys.Escape);

            SceneManager.Instance.AddEntityToSceneOnLayer("Game", 0, keyboardControl);
        }

        private void InitChopper(ECSEngine engine) {
            sm.RegisterSystem("Game", new ChopperControlSystem(engine));

            Entity chopper = EntityFactory.Instance.NewEntityWithTag("Chopper");
            ModelComponent modelComp = new ModelComponent(engine.LoadContent<Model>("models/chopper"), true, false);
            ModelRenderSystem.AddMeshTransform(ref modelComp, 1, Matrix.CreateRotationY(0.2f));
            ModelRenderSystem.AddMeshTransform(ref modelComp, 3, Matrix.CreateRotationY(0.5f));
            ComponentManager.Instance.AddComponentToEntity(chopper, modelComp);

            TransformComponent chopperTransform = new TransformComponent();
            chopperTransform.position = new Vector3(0.0f, 0.0f, 0.0f);
            chopperTransform.vRotation = new Vector3(0, 0, 0);
            chopperTransform.scale = new Vector3(2.5f, 2.5f, 2.5f);
            ComponentManager.Instance.AddComponentToEntity(chopper, chopperTransform);

            ModelBoundingBoxComponent chopperBox = new ModelBoundingBoxComponent(modelComp);
            ComponentManager.Instance.AddComponentToEntity(chopper, chopperBox);

            SceneManager.Instance.AddEntityToSceneOnLayer("Game", 3, chopper);
        }

        private void InitCamera(ECSEngine engine) {
            sm.RegisterSystem("Game", new CameraSystem());

            Entity camera = EntityFactory.Instance.NewEntityWithTag("3DCamera");
            CameraComponent cc = new CameraComponent(engine.GetGraphicsDeviceManager());
            cc.position = new Vector3(0, 20, 60);

            //Use this line instead to see the back rotor rotate, hard to see from behind :)
            //cc.camChasePosition = new Vector3(10f, 20f, 40f);
            cc.camChasePosition = new Vector3(0f, 30f, 70f);

            ComponentManager.Instance.AddComponentToEntity(camera, cc);
            ComponentManager.Instance.AddComponentToEntity(camera, new TransformComponent());
            CameraSystem.SetTargetEntity("Chopper");
            CameraSystem.SetFarClipPlane(ref cc, 500);

            SceneManager.Instance.AddEntityToSceneOnLayer("Game", 6, camera);
        }

        private void InitSkybox(ECSEngine engine) {
            sm.RegisterSystem("Game", new SkyboxSystem());

            Entity skyboxEnt = EntityFactory.Instance.NewEntityWithTag("Skybox");
            SkyboxComponent skybox = new SkyboxComponent(engine.LoadContent<Model>("skyboxes/cube"),
                engine.LoadContent<TextureCube>("skyboxes/Sunset"),
                engine.LoadContent<Effect>("skyboxes/Skybox"));

            ComponentManager.Instance.AddComponentToEntity(skyboxEnt, skybox);

            SceneManager.Instance.AddEntityToSceneOnLayer("Game", 1, skyboxEnt);
        }

        private void InitTerrain(ECSEngine engine) {
            sm.RegisterSystem("Game", new TerrainMapRenderSystem());

            Texture2D terrainTex = engine.LoadContent<Texture2D>("Canyon");
            Texture2D defaultTex = engine.LoadContent<Texture2D>("textures/grasstile");
            Texture2D verticalRoad = engine.LoadContent<Texture2D>("textures/verticalroad");
            Texture2D horizontalRoad = engine.LoadContent<Texture2D>("textures/horizontalroad");

            Entity terrain = EntityFactory.Instance.NewEntityWithTag("Terrain");
            TerrainMapComponent t = new TerrainMapComponent(engine.GetGraphicsDevice(), terrainTex, defaultTex, 10);
            TransformComponent tf = new TransformComponent();

            TerrainMapRenderSystem.LoadHeightMap(ref t, terrainTex, defaultTex, engine.GetGraphicsDevice());

            t.SetTextureToChunk(0, engine.LoadContent<Texture2D>("textures/LTCornerroad"));
            t.SetTextureToChunk(1, verticalRoad);
            t.SetTextureToChunk(2, verticalRoad);
            t.SetTextureToChunk(3, verticalRoad);
            t.SetTextureToChunk(4, verticalRoad);
            t.SetTextureToChunk(5, verticalRoad);
            t.SetTextureToChunk(6, verticalRoad);
            t.SetTextureToChunk(7, verticalRoad);
            t.SetTextureToChunk(8, verticalRoad);
            t.SetTextureToChunk(9, verticalRoad);
            t.SetTextureToChunk(10, horizontalRoad);
            t.SetTextureToChunk(19, horizontalRoad);
            t.SetTextureToChunk(20, horizontalRoad);
            t.SetTextureToChunk(29, horizontalRoad);
            t.SetTextureToChunk(30, horizontalRoad);
            t.SetTextureToChunk(39, horizontalRoad);
            t.SetTextureToChunk(40, horizontalRoad);
            t.SetTextureToChunk(49, horizontalRoad);
            t.SetTextureToChunk(50, horizontalRoad);
            t.SetTextureToChunk(59, horizontalRoad);
            t.SetTextureToChunk(60, horizontalRoad);
            t.SetTextureToChunk(69, horizontalRoad);
            t.SetTextureToChunk(70, horizontalRoad);
            t.SetTextureToChunk(79, horizontalRoad);
            t.SetTextureToChunk(80, horizontalRoad);
            t.SetTextureToChunk(89, horizontalRoad);
            t.SetTextureToChunk(90, engine.LoadContent<Texture2D>("textures/RTCornerroad"));
            t.SetTextureToChunk(99, engine.LoadContent<Texture2D>("textures/RBCornerroad"));
            t.SetTextureToChunk(98, verticalRoad);
            t.SetTextureToChunk(97, verticalRoad);
            t.SetTextureToChunk(96, verticalRoad);
            t.SetTextureToChunk(95, verticalRoad);
            t.SetTextureToChunk(94, verticalRoad);
            t.SetTextureToChunk(93, verticalRoad);
            t.SetTextureToChunk(92, verticalRoad);
            t.SetTextureToChunk(91, verticalRoad);

            tf.world = Matrix.CreateTranslation(0, 0, 0);
            tf.position = Vector3.Zero;

            Entity ModelCounter = EntityFactory.Instance.NewEntity();
            ComponentManager.Instance.AddComponentToEntity(ModelCounter, new ModelCountComponent());
            ComponentManager.Instance.AddComponentToEntity(terrain, t);
            ComponentManager.Instance.AddComponentToEntity(terrain, tf);

            SceneManager.Instance.AddEntityToSceneOnLayer("Game", 2, terrain);
            SceneManager.Instance.AddEntityToSceneOnLayer("Game", 6, ModelCounter);
        }
    }
}
