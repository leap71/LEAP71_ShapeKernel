//
// SPDX-License-Identifier: Apache-2.0
//
// The LEAP 71 ShapeKernel is an open source geometry engine
// specifically for use in Computational Engineering Models (CEM).
//
// For more information, please visit https://leap71.com/shapekernel
// 
// This project is developed and maintained by LEAP 71 - © 2023 by LEAP 71
// https://leap71.com
//
// Computational Engineering will profoundly change our physical world in the
// years ahead. Thank you for being part of the journey.
//
// We have developed this library to be used widely, for both commercial and
// non-commercial projects alike. Therefore, have released it under a permissive
// open-source license.
// 
// The LEAP 71 ShapeKernel is based on the PicoGK compact computational geometry 
// framework. See https://picogk.org for more information.
//
// LEAP 71 licenses this file to you under the Apache License, Version 2.0
// (the "License"); you may not use this file except in compliance with the
// License. You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, THE SOFTWARE IS
// PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED.
//
// See the License for the specific language governing permissions and
// limitations under the License.   
//


using System.Numerics;


namespace Leap71
{
    namespace ShapeKernel
    {
        public class LocalFrame
        {
            protected Vector3 m_vecPosition;
            protected Vector3 m_vecLocalX;
            protected Vector3 m_vecLocalY;
            protected Vector3 m_vecLocalZ;

            /// <summary>
            /// Creates a local frame with default values.
            /// The position will be the origin.
            /// Local x = absolute x direction.
            /// Local y = absolute y direction.
            /// Local z = absolute z direction.
            /// </summary>
            public LocalFrame()
            {
                //default values
                m_vecPosition = new Vector3();
                m_vecLocalZ = Vector3.UnitZ;
                m_vecLocalX = Vector3.UnitX;
                m_vecLocalY = vecGetLocalY(m_vecLocalZ, m_vecLocalX);
            }

            /// <summary>
            /// Creates a local frame with the same axes as the specified base frame, but with a new position.
            /// </summary>
            public LocalFrame(LocalFrame oBaseFrame, Vector3 vecNewPos)
            {
                m_vecPosition = vecNewPos;
                m_vecLocalZ = oBaseFrame.vecGetLocalZ();
                m_vecLocalX = oBaseFrame.vecGetLocalX();
                m_vecLocalY = oBaseFrame.vecGetLocalY();
            }

            /// <summary>
            /// Creates a local frame with the specified position and default axes.
            /// Local x = absolute x direction.
            /// Local y = absolute y direction.
            /// Local z = absolute z direction.
            /// </summary>
            public LocalFrame(Vector3 vecPos)
            {
                m_vecPosition = vecPos;
                m_vecLocalZ = Vector3.UnitZ;
                m_vecLocalX = Vector3.UnitX;
                m_vecLocalY = vecGetLocalY(m_vecLocalZ, m_vecLocalX);
            }

            /// <summary>
            /// Creates a local frame with the specified position and local z.
            /// Local x will be chosen as an arbitary orthogonal direction to local z.
            /// Local y will complement local x and local z for a right-hand system.
            /// </summary>
            public LocalFrame(Vector3 vecPos, Vector3 vecLocalZ)
            {
                //normalize inputs
                if (vecLocalZ.LengthSquared() == 0)
                {
                    throw new Exception("Local Z Coordinate has a length of Zero!");
                }
                vecLocalZ /= vecLocalZ.Length();

                m_vecPosition = vecPos;
                m_vecLocalZ = vecLocalZ;
                m_vecLocalX = VecOperations.vecGetOrthogonalDir(m_vecLocalZ);
                m_vecLocalY = vecGetLocalY(m_vecLocalZ, m_vecLocalX);
            }

            /// <summary>
            /// Creates a local frame with the specified position and local z and local x.
            /// Local y will complement local x and local z for a right-hand system.
            /// </summary>
            public LocalFrame(Vector3 vecPos, Vector3 vecLocalZ, Vector3 vecLocalX)
            {
                //normalize inputs
                if (vecLocalZ.LengthSquared() == 0)
                {
                    throw new Exception("Local Z Coordinate has a length of Zero!");
                }
                if (vecLocalX.LengthSquared() == 0)
                {
                    throw new Exception("Local X Coordinate has a length of Zero!");
                }
                vecLocalZ /= vecLocalZ.Length();
                vecLocalX /= vecLocalX.Length();

                m_vecPosition = vecPos;
                m_vecLocalZ = vecLocalZ;
                m_vecLocalX = vecLocalX;
                m_vecLocalY = vecGetLocalY(m_vecLocalZ, m_vecLocalX);
            }


            //access functions
            public Vector3 vecGetPosition()
            {
                return m_vecPosition;
            }

            public Vector3 vecGetLocalX()
            {
                return m_vecLocalX;
            }

            public Vector3 vecGetLocalY()
            {
                return m_vecLocalY;
            }

            public Vector3 vecGetLocalZ()
            {
                return m_vecLocalZ;
            }


            //utility
            /// <summary>
            /// Returns a local frame that is flipped compared to the specified local frame.
            /// The position remains constant.
            /// The booleans toogle which local axes whould be inverted.
            /// </summary>
            public static LocalFrame oGetInvertFrame(LocalFrame oFrame, bool bMirrorZ, bool bMirrorX)
            {
                Vector3 vecNewLocalZ = oFrame.vecGetLocalZ();
                Vector3 vecNewLocalX = oFrame.vecGetLocalX();
                if (bMirrorZ)
                {
                    vecNewLocalZ = -oFrame.vecGetLocalZ();
                }
                if (bMirrorX)
                {
                    vecNewLocalX = -oFrame.vecGetLocalX();
                }

                LocalFrame oNewFrame = new LocalFrame(oFrame.vecGetPosition(), vecNewLocalZ, vecNewLocalX);
                return oNewFrame;
            }

            /// <summary>
            /// Returns a local frame that is translates in position compared to the specified local frame.
            /// The axes remain constant.
            /// </summary>
            public static LocalFrame oGetTranslatedFrame(LocalFrame oFrame, Vector3 vecTranslate)
            {
                Vector3 vecNewPosition = oFrame.vecGetPosition() + vecTranslate;
                LocalFrame oNewFrame = new LocalFrame(vecNewPosition, oFrame.vecGetLocalZ(), oFrame.vecGetLocalX());
                return oNewFrame;
            }

            /// <summary>
            /// Compliments local z and local x according to a right-hand system.
            /// </summary>
            public static Vector3 vecGetLocalY(Vector3 vecLocalZ, Vector3 vecLocalX)
            {
                Vector3 vecLocalY = Vector3.Cross(vecLocalZ, vecLocalX);
                return vecLocalY;
            }
        }
    }
}