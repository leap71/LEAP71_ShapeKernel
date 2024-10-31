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
            /// The function iterates across all active voxels.
            /// All active voxel positions are accumulated and devided by the voxel count.
            /// The centre of gravity is measured in units of mm.
            /// </summary>
			public static Vector3 vecGetCentreOfGravity(Voxels oVoxels)
            {
                VectorField oGradientField  = new(oVoxels);
                ScalarField oSDField        = new(oVoxels);
                oVoxels.CalculateProperties(out float fVolume, out BBox3 oBox);
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
                            if (oGradientField.bGetValue(vecPt, out Vector3 vecVal))
                            {
                                if (oSDField.bGetValue(vecPt, out float fSDVal))
                                {
                                    if (fSDVal < 0)
                                    {
                                        vecCoG      += vecPt;
                                        fCounter    += 1f;
                                    }
                                }
                            }
                        }
                    }
                }
                vecCoG /= fCounter;
                return vecCoG;
            }

            /// <summary>
            /// Returns the inertia tensor of a voxelfield with respect to a reference frame.
            /// The function iterates across all active voxels.
            /// All active voxels are treated as idealized point masses.
            /// All active voxel contributions to the inertia tensor are accumulated and devided by the mass.
            /// The voxelfield's density is specified in kg/m3.
            /// The density is assumed to be homogeneously distributed.
            /// The resulting 3x3 matrix holds components of moments of inertia in units of kg * m2.
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

                // iterate across all active voxels
                // treat active voxel as point mass
                VectorField oGradientField  = new(oVoxels);
                ScalarField oSDField        = new(oVoxels);
                oVoxels.CalculateProperties(out float fVolume, out BBox3 oBox);
                oBox.Grow(1f);
                Vector3 vecSize             = oBox.vecSize();
                float fStep                 = Library.fVoxelSizeMM;
                float fCounter              = 0f;

                // count active voxels
                for (float fX = 0f; fX < vecSize.X; fX += fStep)
                {
                    for (float fY = 0f; fY < vecSize.Y; fY += fStep)
                    {
                        for (float fZ = 0f; fZ < vecSize.Z; fZ += fStep)
                        {
                            Vector3 vecPt  = oBox.vecMin + new Vector3(fX, fY, fZ);
                            if (oGradientField.bGetValue(vecPt, out Vector3 vecVal))
                            {
                                if (oSDField.bGetValue(vecPt, out float fSDVal))
                                {
                                    if (fSDVal < 0)
                                    {
                                        fCounter               += 1f;
                                    }
                                }
                            }
                        }
                    }
                }

                float dVoxelVolume  = Library.fVoxelSizeMM * Library.fVoxelSizeMM * Library.fVoxelSizeMM;
                float fVoxelVolume  = fCounter * dVoxelVolume;
                float fVolumeFactor = fVolume / fVoxelVolume;

                for (float fX = 0f; fX < vecSize.X; fX += fStep)
                {
                    for (float fY = 0f; fY < vecSize.Y; fY += fStep)
                    {
                        for (float fZ = 0f; fZ < vecSize.Z; fZ += fStep)
                        {
                            Vector3 vecPt  = oBox.vecMin + new Vector3(fX, fY, fZ);
                            if (oGradientField.bGetValue(vecPt, out Vector3 vecVal))
                            {
                                if (oSDField.bGetValue(vecPt, out float fSDVal))
                                {
                                    if (fSDVal < 0)
                                    {
                                        float dVoxelMass        = fDensity * fVolumeFactor * dVoxelVolume / MathF.Pow(10f, 9f);     // kg
                                        double[,] matIncrement  = matGetMomentOfInertia(vecPt, oRefFrame, dVoxelMass);              // kg * m2
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
                float fHeight               = 5f;
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

                // analytical formulas for a "chubby" cylinder
                float fAnalyticIxx          = 0.25f * fAnalyticMass * fRadius * fRadius;
                float fAnalyticIyy          = 0.25f * fAnalyticMass * fRadius * fRadius;
                float fAnalyticIzz          = 0.5f * fAnalyticMass * fRadius * fRadius;
                if (fHeight > 5f * fRadius)
                {
                    // analytical formulas for a "skinny" cylinder
                    fAnalyticIxx            = 1f / 12f * fAnalyticMass * (3f * fRadius * fRadius + fHeight * fHeight);
                    fAnalyticIyy            = 1f / 12f * fAnalyticMass * (3f * fRadius * fRadius + fHeight * fHeight);
                    fAnalyticIzz            = 0.5f * fAnalyticMass * fRadius * fRadius;
                }
              
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