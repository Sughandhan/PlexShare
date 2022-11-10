﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using System.Windows.Shapes;
using PlexShareWhiteboard.BoardComponents;

namespace PlexShareWhiteboard
{
    public partial class WhiteBoardViewModel
    {
        // this is not for creation but for updating a created shape as we are using lastid.
        public ShapeItem UpdateShape(Point start, Point end, string name, ShapeItem oldShape)
        {
            Rect boundingBox = new (start, end);
            Geometry geometry;
            geometry = new RectangleGeometry(boundingBox);

            if (name == "EllipseGeometry")
                geometry = new EllipseGeometry(boundingBox);

            ShapeItem newShape = new()
            {
                Geometry = geometry,
                Fill = oldShape.Fill,
                Stroke = oldShape.Stroke,
                ZIndex = oldShape.ZIndex,
                AnchorPoint = start,
                Id = oldShape.Id
            };

            for (int i = 0; i < ShapeItems.Count; i++)
            {

                if (ShapeItems[i].Id == oldShape.Id)
                {
                    ShapeItems[i] = newShape;
                }
            }

            return newShape;
        }
        public ShapeItem CreateShape(Point start, Point end, string name, String id)
        {
            Rect boundingBox = new (start, end);
            Geometry geometry;
            geometry = new RectangleGeometry(boundingBox);

            if (name == "EllipseGeometry")
                geometry = new EllipseGeometry(boundingBox);

            ShapeItem newShape = new()
            {
                Geometry = geometry,
                Fill = fillBrush,
                Stroke = strokeBrush,
                ZIndex = currentZIndex,
                AnchorPoint = start,
                Id = id
            };

            for (int i = 0; i < ShapeItems.Count; i++)
            {

                if (ShapeItems[i].Id == id)
                {
                    ShapeItems[i] = newShape;
                }
            }

            return newShape;
        }

        public void DeleteShape(Point a)
        {
            int tempZIndex = -1;
            ShapeItem toDelete = null;

            for (int i = ShapeItems.Count - 1; i >= 0; i--)
            {
                Geometry Child = ShapeItems[i].Geometry;

                if (ShapeItems[i].ZIndex > tempZIndex && Child.FillContains(a))
                {
                    tempZIndex = ShapeItems[i].ZIndex;
                    toDelete = ShapeItems[i];
                }
                else if (ShapeItems[i].ZIndex > tempZIndex && Child.GetType().Name == "PathGeometry" &&
                    PointInsideRect(Child.Bounds, a))
                {
                    tempZIndex = ShapeItems[i].ZIndex;
                    toDelete = ShapeItems[i];
                }
            }

            if (toDelete != null)
            {
                lastShape = toDelete;
                ShapeItems.Remove(toDelete);
            }
        }

        public void TransformShape(ShapeItem shape, double newXLen, double newYLen, int signX, int signY)
        {
            Rect boundingBox = shape.Geometry.Bounds;
            double ratio = Math.Abs(boundingBox.Width / boundingBox.Height);
            if (ratio < Math.Abs(newXLen / newYLen))
                newXLen = Math.Abs(ratio * newYLen) * signX;
            else
                newYLen = Math.Abs(newXLen / ratio) * signY;

            double newX = select.initialSelectionPoint.X + newXLen;
            double newY = select.initialSelectionPoint.Y + newYLen;
            Point p1 = new (boundingBox.X, boundingBox.Y);
            Point p2 = new (boundingBox.X + boundingBox.Width, boundingBox.Y + boundingBox.Height);

            int boxNumber = select.selectBox;
                
            if (boxNumber == 1 && signX * signY > 0)
            {
                p1 = new Point(newX, newY);
                p2 = new Point(boundingBox.X + boundingBox.Width, boundingBox.Y + boundingBox.Height);
            }
            else if (boxNumber == 2 && signX * signY < 0)
            {
                p1 = new Point(boundingBox.X, newY);
                p2 = new Point(newX, boundingBox.Y + boundingBox.Height);
            }
            else if (boxNumber == 3 && signY * signX < 0)
            {
                p1 = new Point(newX, boundingBox.Y);
                p2 = new Point(boundingBox.X + boundingBox.Width, newY);
            }
            else if (boxNumber == 4 && signX * signY > 0)
            {
                p1 = new Point(boundingBox.X, boundingBox.Y);
                p2 = new Point(newX, newY);
            }

            if (boxNumber > 0)
            {
                shape = UpdateShape(p1, p2, shape.Geometry.GetType().Name, shape);
                lastShape = shape;
                HighLightIt(shape.Geometry.Bounds);
            }
        }

        public void DimensionChangingShape(Point a, ShapeItem shape)
        {
            Rect boundingBox = shape.Geometry.Bounds;
            Point p1 = new(boundingBox.X, boundingBox.Y);
            Point p2 = new(boundingBox.X + boundingBox.Width, boundingBox.Y + boundingBox.Height);
            int boxNumber = select.selectBox;

            if (boxNumber == 5)
            {
                p1 = new Point(boundingBox.X, a.Y);
                p2 = new Point(boundingBox.X + boundingBox.Width, boundingBox.Y + boundingBox.Height);
            }
            else if (boxNumber == 6)
            {
                p1 = new Point(a.X, boundingBox.Y);
                p2 = new Point(boundingBox.X + boundingBox.Width, boundingBox.Y + boundingBox.Height);
            }
            else if (boxNumber == 7)
            {
                p1 = new Point(boundingBox.X, boundingBox.Y);
                p2 = new Point(a.X, boundingBox.Y + boundingBox.Height);
            }
            else if (boxNumber == 8)
            {
                p1 = new Point(boundingBox.X, boundingBox.Y);
                p2 = new Point(boundingBox.X + boundingBox.Width, a.Y);
            }

            if (boxNumber > 0)
            {
                shape = UpdateShape(p1, p2, shape.Geometry.GetType().Name, shape);
                lastShape = shape;
                //HighLightIt(shape.Geometry.Bounds);
            }
        }
        public void TranslatingShape(ShapeItem shape, Point p1, Point p2)
        {
            lastShape = UpdateShape(p1, p2, shape.Geometry.GetType().Name, shape);
            HighLightIt(p1, p2);
        }
    }
}
