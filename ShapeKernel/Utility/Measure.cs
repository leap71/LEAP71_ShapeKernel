//
// SPDX-License-Identifier: Apache-2.0
//
// The LEAP 71 ShapeKernel is an open source geometry engine
// specifically for use in Computational Engineering Models (CEM).
//
// For more information, please visit https://leap71.com/shapekernel
// 
// This project is developed and maintained by LEAP 71 - © 2024 by LEAP 71
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
				Mesh oMesh = new Mesh(oVoxels);
				return fGetSurfaceArea(oMesh);
            }

			/// <summary>
            /// Calculates the area of each mesh triangle.
            /// Adds all triangle areas to obtain the final result.
            /// The area is measured in mm^2.
            /// </summary>
            public static float fGetSurfaceArea(Mesh msh)
			{
				int nTriangleCount	= msh.nTriangleCount();

				float fArea = 0;
                
				for (int n = 0; n < nTriangleCount; n++)
				{
                    msh.GetTriangle(    n, 
                                        out Vector3 vecA, 
                                        out Vector3 vecB, 
                                        out Vector3 vecC);

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
            /// Returns the centre of gravity of a voxelfield.
            /// The function samples the bounding box on the voxel grid and counts every
            /// point whose signed distance is zero or negative, i.e. inside the body.
            /// The return value of ScalarField.bGetValue is deliberately ignored: it only
            /// says whether a value is stored at that point, and values are stored in the
            /// narrow band around the surface only. Filtering on it would weigh a shell
            /// instead of the solid.
            /// The centre of gravity is measured in units of mm.
            /// </summary>
			public static Vector3 vecGetCentreOfGravity(Voxels oVoxels)
            {
                ScalarField oSDField        = new(oVoxels);
                BBox3 oBox                  = oVoxels.oCalculateBoundingBox();
                oBox.Grow(1f);
                Vector3 vecSize             = oBox.vecSize();
                float fStep                 = Library.fVoxelSizeMM;
                Vector3 vecCoG              = new Vector3();
                float fCounter              = 0f;


                for (float fX = 0f; fX < vecSize.X; fX += fStep)
                {
                    for (float fY = 0f; fY < vecSize.Y; fY += fStep)
                    {
                        for (float fZ = 0f; fZ < vecSize.Z; fZ += fStep)
                        {
                            Vector3 vecPt  = oBox.vecMin + new Vector3(fX, fY, fZ);
                            oSDField.bGetValue(vecPt, out float fSDVal);
                            if (fSDVal <= 0f)
                            {
                                vecCoG      += vecPt;
                                fCounter    += 1f;
                            }
                        }
                    }
                }
                vecCoG /= fCounter;
                return vecCoG;
            }

            /// <summary>
            /// Returns the inertia tensor of a voxelfield with respect to a reference frame.
            /// The function samples the bounding box on the voxel grid and treats every
            /// point whose signed distance is zero or negative as a point mass.
            /// The return value of ScalarField.bGetValue is deliberately ignored: it only
            /// says whether a value is stored at that point, and values are stored in the
            /// narrow band around the surface only. Filtering on it would weigh a shell
            /// instead of the solid and overestimate the inertia by up to 2/3 : 2/5.
            /// The total mass is density times the measured volume, distributed evenly
            /// over the counted points.
            /// The voxelfield's density is specified in kg/m3.
            /// The density is assumed to be homogeneously distributed.
            /// The resulting 3x3 matrix holds components of moments of inertia in units of kg * mm2
            /// (positions are in mm, mass in kg).
            /// </summary>
            public static double[,] matGetMomentOfInertia(Voxels oVoxels, LocalFrame oRefFrame, float fDensity)
            {
                // initialize
                float fMeasureMass          = 0;
                double[,] matInertiaTensor  = new double[3, 3]   {
                                                                    { 0f, 0f, 0f },
                                                                    { 0f, 0f, 0f },
                                                                    { 0f, 0f, 0f }
                                                                };

                ScalarField oSDField        = new(oVoxels);
                float fVolume               = fGetVolume(oVoxels);
                BBox3 oBox                  = oVoxels.oCalculateBoundingBox();
                oBox.Grow(1f);
                Vector3 vecSize             = oBox.vecSize();
                float fStep                 = Library.fVoxelSizeMM;
                float fCounter              = 0f;

                // count points inside the body
                for (float fX = 0f; fX < vecSize.X; fX += fStep)
                {
                    for (float fY = 0f; fY < vecSize.Y; fY += fStep)
                    {
                        for (float fZ = 0f; fZ < vecSize.Z; fZ += fStep)
                        {
                            Vector3 vecPt  = oBox.vecMin + new Vector3(fX, fY, fZ);
                            oSDField.bGetValue(vecPt, out float fSDVal);
                            if (fSDVal <= 0f)
                            {
                                fCounter += 1f;
                            }
                        }
                    }
                }

                if (fCounter == 0f)
                    return matInertiaTensor;

                // distribute the measured volume evenly over the counted points
                float dVoxelMass = fDensity * (fVolume / fCounter) / MathF.Pow(10f, 9f);     // kg

                for (float fX = 0f; fX < vecSize.X; fX += fStep)
                {
                    for (float fY = 0f; fY < vecSize.Y; fY += fStep)
                    {
                        for (float fZ = 0f; fZ < vecSize.Z; fZ += fStep)
                        {
                            Vector3 vecPt  = oBox.vecMin + new Vector3(fX, fY, fZ);
                            oSDField.bGetValue(vecPt, out float fSDVal);
                            if (fSDVal <= 0f)
                            {
                                double[,] matIncrement  = matGetMomentOfInertia(vecPt, oRefFrame, dVoxelMass);              // kg * mm2
                                fMeasureMass            += dVoxelMass;

                                // accumulate inertia matrices
                                for (int k = 0; k < 3; k++)
                                {
                                    for (int l = 0; l < 3; l++)
                                    {
                                        matInertiaTensor[k, l] += matIncrement[k, l];
                                    }
                                }
                            }
                        }
                    }
                }

                // compare total mass against accumulated voxel mass
                float fMass = fDensity * fVolume / MathF.Pow(10f, 9f);
        
                return matInertiaTensor;
            }

            /// <summary>
            /// Returns the inertia tensor of an idealized point mass with respect to a reference frame.
            /// General Formula: I = m * r^2.
            /// The point's mass is specified in kg.
            /// The resulting 3x3 matrix holds components of moments of inertia in units of kg * mm2.
            /// </summary>
            protected static double[,] matGetMomentOfInertia(Vector3 vecPt, LocalFrame oRefFrame, float fMass)
            {
                // get coordinate representation relative to reference frame
                Vector3 vecRel      = VecOperations.vecExpressPointInFrame(oRefFrame, vecPt);
                float fX            = vecRel.X;
                float fY            = vecRel.Y;
                float fZ            = vecRel.Z;

                // components of inertia tensor
                float fIxx          = fMass * (fY * fY + fZ * fZ);
                float fIyy          = fMass * (fX * fX + fZ * fZ);
                float fIzz          = fMass * (fX * fX + fY * fY);
                float fIxy          = fMass * (-fX * fY);
                float fIxz          = fMass * (-fX * fZ);
                float fIyx          = fMass * (-fY * fX);
                float fIyz          = fMass * (-fY * fZ);
                float fIzx          = fMass * (-fZ * fX);
                float fIzy          = fMass * (-fZ * fY);

                // write inertia tensor from components as a 3x3 matrix
                double[,] matInertiaTensor = new double[3, 3]   {
                                                                    { fIxx, fIxy, fIxz },
                                                                    { fIyx, fIyy, fIyz },
                                                                    { fIzx, fIzy, fIzz }
                                                                };
                return matInertiaTensor;
            }

			/// <summary>
			/// Runs a test to compare the calculated surface area of a sphere against the measured value.
			/// </summary>
			public static void TestSurfaceMesurement()
			{
				float fRadius				= 20f;
				BaseSphere oSphere			= new BaseSphere(new LocalFrame(), fRadius);
				Voxels voxSphere			= oSphere.voxConstruct();

				float fMeasuredSurfaceArea	= fGetSurfaceArea(voxSphere);
				float fAnalyticSurfaceArea	= 4f * MathF.PI * fRadius * fRadius;

				Library.Log($"Measured surface area = {fMeasuredSurfaceArea} mm^2.");
                Library.Log($"Expected surface area = {fAnalyticSurfaceArea} mm^2.");
            }

            /// <summary>
            /// Runs a test to compare the calculated volume of a sphere against the measured value.
            /// </summary>
            public static void TestVolumeMesurement()
			{
				float fRadius				= 20f;
				BaseSphere oSphere			= new BaseSphere(new LocalFrame(), fRadius);
				Voxels voxSphere			= oSphere.voxConstruct();

				float fMeasuredVolume		= fGetVolume(voxSphere);
				float fAnalyticVolume		= 4f / 3f * MathF.PI * fRadius * fRadius * fRadius;

				Library.Log($"Measured volume = {fMeasuredVolume} mm^3.");
                Library.Log($"Expected volume = {fAnalyticVolume} mm^3.");
            }

            /// <summary>
            /// Runs a test to compare the calculated centre of gravity of a cylinder against the measured value.
            /// </summary>
            public static void TestCentreOfGravityMesurement()
            {
                float fHeight               = 40f;
                float fRadius               = 30f;
                LocalFrame oFrame           = new LocalFrame(new Vector3(30, 36, -12), new Vector3(2f, -1f, 1f).Normalize());
                BaseCylinder oCyl           = new BaseCylinder(oFrame, fHeight, fRadius);
                Voxels vox                  = oCyl.voxConstruct();

                Vector3 vecMeasuredCoG      = vecGetCentreOfGravity(vox);
                Vector3 vecAnalyticCoG      = oFrame.vecGetPosition() + 0.5f * fHeight * oFrame.vecGetLocalZ();

                Library.Log($"Measured centre of gravity = {vecMeasuredCoG} mm.");
                Library.Log($"Expected centre of gravity = {vecAnalyticCoG} mm.");
            }

            /// <summary>
            /// Runs a test to compare the calculated inertia tensor of a cylinder against the measured value.
            /// </summary>
            public static void TestInertiaTensorMesurement()
            {
                float fHeight               = 40f;
                float fRadius               = 50f;
                float fDensity              = 7000f;
                LocalFrame oFrame           = new LocalFrame(new Vector3(30, 36, -12), new Vector3(2f, -1f, 1f).Normalize());
                BaseCylinder oCyl           = new BaseCylinder(oFrame, fHeight, fRadius);
                Voxels vox                  = oCyl.voxConstruct();

                // measured cylinder
                Vector3 vecMeasuredCoG      = vecGetCentreOfGravity(vox);
                float fMeasuredVolume       = fGetVolume(vox);                                      // mm3
                float fMeasuredMass         = fDensity * fMeasuredVolume / MathF.Pow(10f, 9f);      // kg

                // reference frame witho on CoG and main axes
                LocalFrame oRefFrame        = new LocalFrame(vecMeasuredCoG, oFrame.vecGetLocalZ(), oFrame.vecGetLocalX());
                double[,] matInertiaTensor  = matGetMomentOfInertia(vox, oRefFrame, fDensity);
                float fMeasuredIxx          = (float)matInertiaTensor[0, 0];
                float fMeasuredIyy          = (float)matInertiaTensor[1, 1];
                float fMeasuredIzz          = (float)matInertiaTensor[2, 2];

                // analytic cylinder
                Vector3 vecAnalyticCoG      = oFrame.vecGetPosition() + 0.5f * fHeight * oFrame.vecGetLocalZ();
                float fAnalyticVolume       = MathF.PI * fRadius * fRadius * fHeight;               // mm3
                float fAnalyticMass         = fDensity * fAnalyticVolume / MathF.Pow(10f, 9f);      // kg

                // solid cylinder, axes through the centre of gravity
                // (a cylinder of a few voxels height would fit entirely into the narrow band
                // and could not tell a shell from a solid, so the test uses a full body)
                float fAnalyticIxx          = 1f / 12f * fAnalyticMass * (3f * fRadius * fRadius + fHeight * fHeight);
                float fAnalyticIyy          = 1f / 12f * fAnalyticMass * (3f * fRadius * fRadius + fHeight * fHeight);
                float fAnalyticIzz          = 0.5f * fAnalyticMass * fRadius * fRadius;
              
                Library.Log($"Measured Mass = {fMeasuredMass} kg.");
                Library.Log($"Measured Centre of Gravity = {vecMeasuredCoG} mm.");
                Library.Log($"Measured Ixx = {fMeasuredIxx} kg * mm2.");
                Library.Log($"Measured Iyy = {fMeasuredIyy} kg * mm2.");
                Library.Log($"Measured Izz = {fMeasuredIzz} kg * mm2.");
                Library.Log($"\n");
                Library.Log($"Expected Mass = {fAnalyticMass} kg.");
                Library.Log($"Expected Centre of Gravity = {vecAnalyticCoG} mm.");
                Library.Log($"Expected Ixx = {fAnalyticIxx} kg * mm2.");
                Library.Log($"Expected Iyy = {fAnalyticIyy} kg * mm2.");
                Library.Log($"Expected Izz = {fAnalyticIzz} kg * mm2.");
            }
        }
	}
}