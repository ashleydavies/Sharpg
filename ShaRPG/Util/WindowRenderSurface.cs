﻿using System;
using SFML.Graphics;
using SFML.System;
using ShaRPG.Camera;
using ShaRPG.Util.Coordinate;

namespace ShaRPG.Util {
    internal class WindowRenderSurface : IRenderSurface {
        private readonly RenderWindow _window;
        private Vector2f _screenRenderOffset = new Vector2f(0, 0);

        public WindowRenderSurface(RenderWindow window) {
            _window = window;
            Size = new Vector2I((int) _window.Size.X, (int) _window.Size.Y);
        }

        public Vector2I Size { get; set; }

        public void Render(IDrawable drawable, GameCoordinate position) {
            drawable.Draw(_window, position);
        }

        public void Render(IDrawable drawable, ScreenCoordinate position) {
            View view = _window.GetView();
            View defView = _window.DefaultView;
            defView.Move(_screenRenderOffset);
            _window.SetView(defView);
            drawable.Draw(_window, new GameCoordinate(position.X, position.Y));
            _window.SetView(view);
        }

        public void AddRenderOffset(Vector2I offset) {
            _screenRenderOffset -= new Vector2f(offset.X, offset.Y);
        }

        public void SubtractRenderOffset(Vector2I offset) {
            _screenRenderOffset += new Vector2f(offset.X, offset.Y);
        }

        public void SetCamera(ICamera gameCamera) => _window.SetView(new View(
                                                                         new Vector2f(gameCamera.Center.X,
                                                                                      gameCamera.Center.Y),
                                                                         new Vector2f(Size.X / gameCamera.Scale.X,
                                                                                      Size.Y / gameCamera.Scale.Y)
                                                                     ));

    }
}
