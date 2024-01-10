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
		public partial class Measure
		{
            /// <summary>
            /// Calculates the volume by counting the number of voxels.
            /// The volume is measured in mm^3.
            /// </summary>
            public static float fGetVolume(Voxels oVoxels)
            {
                oVoxels.CalculateProperties(out float fVolume, out BBox3 oBBox);
                return fVolume;
            }

            /// <summary>
            /// Converts voxelfield into mesh.
            /// Calculates the area of each mesh triangle.
            /// Adds all triangle areas to obtain the final result.
            /// The area is measured in mm^2.
            /// </summary>
            public static float fGetSurfaceArea(Voxels oVoxels)
			{
				Mesh oMesh			= new Mesh(oVoxels);
				int nTriangleCount	= oMesh.nTriangleCount();

				float fArea			= 0;
				for (int i = 0; i < nTriangleCount; i++)
				{
					Triangle oTri	= oMesh.oTriangleAt(i);
					int nIndexA		= oTri.A;
                    int nIndexB		= oTri.B;
                    int nIndexC		= oTri.C;
					Vector3 vecA	= oMesh.vecVertexAt(nIndexA);
                    Vector3 vecB	= oMesh.vecVertexAt(nIndexB);
                    Vector3 vecC	= oMesh.vecVertexAt(nIndexC);

					fArea += fGetTriangleArea(vecA, vecB, vecC);
                }
				return fArea;
			}

            /// <summary>
            /// Returns the area for an individual triangle.
            /// The area is measured in mm^2.
            /// </summary>
            public static float fGetTriangleArea(Vector3 vecA, Vector3 vecB, Vector3 vecC)
			{
				Vector3 vecSideAB	= vecB - vecA;
                Vector3 vecSideAC	= vecC - vecA;
				float fArea			= 0.5f * Vector3.Cross(vecSideAB, vecSideAC).Length();
				return fArea;
            }

			/// <summary>
			/// Runs a test to compare the calculated surface area of a sphere against the measured surface area.
			/// </summary>
			public static void TestSurfaceMesurement()
			{
				float fRadius				= 20;
				BaseSphere oSphere			= new BaseSphere(new LocalFrame(), fRadius);
				Voxels voxSphere			= oSphere.voxConstruct();

				float fMeasuredSurfaceArea	= fGetSurfaceArea(voxSphere);
				float fAnalyticSurfaceArea	= 4f * MathF.PI * fRadius * fRadius;

				Library.Log($"Measured surface area = {fMeasuredSurfaceArea} mm^2.");
                Library.Log($"Expected surface area = {fAnalyticSurfaceArea} mm^2.");
            }

            /// <summary>
            /// Runs a test to compare the calculated volume of a sphere against the measured volume.
            /// </summary>
            public static void TestVolumeMesurement()
			{
				float fRadius				= 20;
				BaseSphere oSphere			= new BaseSphere(new LocalFrame(), fRadius);
				Voxels voxSphere			= oSphere.voxConstruct();

				float fMeasuredVolume		= fGetVolume(voxSphere);
				float fAnalyticVolume		= 4f / 3f * MathF.PI * fRadius * fRadius * fRadius;

				Library.Log($"Measured volume = {fMeasuredVolume} mm^3.");
                Library.Log($"Expected volume = {fAnalyticVolume} mm^3.");
            }
		}
	}
}