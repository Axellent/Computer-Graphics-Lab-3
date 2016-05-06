using System;
using System.Collections.Generic;
using GameEngine;

namespace NAJ_Lab3
{
    class Engine : ECSEngine
    {
        public override void InitialiseContent()
        {
        }

        public override void Initialise()
        {
            new InitGame(this);
            new InitHouses(this);
            SceneManager.Instance.SetActiveScene("Game");
        }
    }
}
