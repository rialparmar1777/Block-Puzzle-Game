using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProject.Interfaces
{

    public interface IScene
    {
        void Update();
        void Draw(SpriteBatch spriteBatch);

        void OnSceneEnabled();
        void OnSceneDisabled();
    }
   
}
