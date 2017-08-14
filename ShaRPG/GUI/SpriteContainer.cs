﻿using ShaRPG.Util;
using ShaRPG.Util.Coordinate;

namespace ShaRPG.GUI {
    public class SpriteContainer : AbstractGuiComponent {
        public override int Height => _sprite.Height;
        public override int Width => _sprite.Width;
        private readonly Sprite _sprite;
        private readonly Alignment _alignment;

        private ScreenCoordinate position => _alignment == Alignment.Left
                                                 ? new ScreenCoordinate(0, 0)
                                                 : (_alignment == Alignment.Right
                                                        ? new ScreenCoordinate(Parent.Width - Width, 0)
                                                        : new ScreenCoordinate(Parent.Width / 2 - Width / 2, 0));

        public SpriteContainer(Sprite sprite, Alignment alignment = Alignment.Left) {
            _sprite = sprite;
            _alignment = alignment;
        }

        public override void Render(IRenderSurface renderSurface) {
            renderSurface.Render(_sprite, position);
        }

        public override void Reflow() { }
    }
}