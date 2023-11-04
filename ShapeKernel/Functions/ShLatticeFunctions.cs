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
using PicoGK;


namespace Leap71
{
    namespace ShapeKernel
    {
        public partial class Sh
        {
            /// <summary>
            /// Creates a lattice from a point list / spline.
            /// </summary>
            public static Lattice latFromLine(List<Vector3> aPoints, float fBeam)
            {
                Lattice oLattice = new Lattice();
                for (int i = 1; i < aPoints.Count; i++)
                {
                    oLattice.AddBeam(aPoints[i - 1], fBeam, aPoints[i], fBeam, true);
                }
                return oLattice;
            }

            /// <summary>
            /// Adds a point list / spline to an existing lattice.
            /// </summary>
            public static void AddLine(ref Lattice oLattice, List<Vector3> aPoints, float fBeam)
            {
                for (int i = 1; i < aPoints.Count; i++)
                {
                    oLattice.AddBeam(aPoints[i - 1], fBeam, aPoints[i], fBeam, true);
                }
            }

            /// <summary>
            /// Creates a node-only lattice from a point cloud.
            /// The points will not be connected.
            /// </summary>
            public static Lattice latFromPoints(List<Vector3> aPoints, float fBeam)
            {
                Lattice oLattice = new Lattice();
                for (int i = 0; i < aPoints.Count; i++)
                {
                    oLattice.AddSphere(aPoints[i], fBeam);
                }
                return oLattice;
            }

            /// <summary>
            /// Creates a lattice from multiple point lists / splines.
            /// </summary>
            public static Lattice latFromEdges(List<List<Vector3>> aEdges, float fBeam)
            {
                Lattice oLattice = new Lattice();
                for (int i = 0; i < aEdges.Count; i++)
                {
                    List<Vector3> aPoints = aEdges[i];
                    for (int j = 1; j < aPoints.Count; j++)
                    {
                        oLattice.AddBeam(aPoints[j - 1], fBeam, aPoints[j], fBeam, true);
                    }
                }
                return oLattice;
            }

            /// <summary>
            /// Creates a node-only lattice from a point.
            /// </summary>
            public static Lattice latFromPoint(Vector3 vecPt, float fBeam)
            {
                Lattice oLattice = new Lattice();
                oLattice.AddSphere(vecPt, fBeam);
                return oLattice;
            }

            /// <summary>
            /// Creates a lattice from a grid.
            /// </summary>
            public static Lattice latFromGrid(List<List<Vector3>> aGrid, float fBeam)
            {
                Lattice oLattice = new Lattice();
                for (int i = 0; i < aGrid.Count; i++)
                {
                    oLattice = latFromLine(aGrid[i], fBeam);
                }
                aGrid = GridOperations.aGetInverseGrid(aGrid);
                for (int i = 0; i < aGrid.Count; i++)
                {
                    List<Vector3> aPoints = aGrid[i];
                    for (int j = 1; j < aPoints.Count; j++)
                    {
                        oLattice.AddBeam(aPoints[j - 1], fBeam, aPoints[j], fBeam, true);
                    }
                }
                return oLattice;
            }

            /// <summary>
            /// Creates a lattice from a beam.
            /// Beam has a constant radius.
            /// Beam has rounded end caps.
            /// </summary>
            public static Lattice latFromBeam(Vector3 vecPt1, Vector3 vecPt2, float fBeam, bool bRounded)
            {
                Lattice oLattice = new Lattice();
                oLattice.AddBeam(vecPt1, fBeam, vecPt2, fBeam, bRounded);
                return oLattice;
            }

            /// <summary>
            /// Creates a lattice from a beam.
            /// Beam has a variable radius.
            /// Beam has rounded end caps.
            /// </summary>
            public static Lattice latFromBeam(Vector3 vecPt1, Vector3 vecPt2, float fBeam1, float fBeam2, bool bRounded)
            {
                Lattice oLattice = new Lattice();
                oLattice.AddBeam(vecPt1, fBeam1, vecPt2, fBeam2, bRounded);
                return oLattice;
            }
        }
    }
}