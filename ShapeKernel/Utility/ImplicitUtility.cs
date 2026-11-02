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
        public class ImplicitGyroid : IImplicit
		{
            protected float         m_fFrequencyScale;
			protected float		    m_fThicknessRatio;

            /// <summary>
            /// Helper class for an implicit gyroid pattern.
            /// The unit size in mm will determine the length after which the pattern repreats.
            /// The thickness ratio is a measure (!) for the wall thickness.
            /// Use fGetThicknessRatio function to determine the correct ratio value
            /// to meet a target physical wall thickness in mm.
            /// </summary>
            public ImplicitGyroid(float fUnitSize, float fThicknessRatio)
			{
                m_fFrequencyScale   = (2f * MathF.PI) / fUnitSize;
                m_fThicknessRatio   = fThicknessRatio;
            }

            /// <summary>
            /// Function to determine the correct gyroid thickness ratio value
            /// to meet a target physical wall thickness in mm.
            /// </summary>
            public static float fGetThicknessRatio(float fWallThickness, float fUnitSize)
            {
                float fRefWallRatio = fWallThickness * 10f / fUnitSize;
                return fRefWallRatio;
            }

            public float fSignedDistance(in Vector3 vecPt)
			{
                double dX = vecPt.X;
                double dY = vecPt.Y;
                double dZ = vecPt.Z;

                //calculate the gyroid surface equation
                double fDist =   Math.Sin(m_fFrequencyScale * dX) * Math.Cos(m_fFrequencyScale * dY) +
                                 Math.Sin(m_fFrequencyScale * dY) * Math.Cos(m_fFrequencyScale * dZ) +
                                 Math.Sin(m_fFrequencyScale * dZ) * Math.Cos(m_fFrequencyScale * dX);

                //apply thickness to the gyroid surface
                return (float)(Math.Abs(fDist) - 0.5f * m_fThicknessRatio);
            }
		}

        public class ImplicitSphere : IImplicit
        {
			protected Vector3	m_vecCentre;
			protected float		m_fRadius;

            /// <summary>
            /// Helper class for an implicit sphere.
            /// </summary>
            public ImplicitSphere(Vector3 vecCentre, float fRadius)
            {
				m_vecCentre = vecCentre;
				m_fRadius	= fRadius;
            }

            public float fSignedDistance(in Vector3 vecPt)
            {
                return (vecPt - m_vecCentre).Length() - m_fRadius;
            }
        }

        public class ImplicitGenus : IImplicit
        {
            protected float m_fGap;

            /// <summary>
            /// Helper class for an implicit genus.
            /// https://en.wikipedia.org/wiki/Implicit_surface
            /// The sepcified gap controls the size of the hole in center.
            /// </summary>
            public ImplicitGenus(float fGap)
            {
                m_fGap = fGap;
            }

            public float fSignedDistance(in Vector3 vecPt)
            {
                return  (float)(2 * vecPt.Y * (vecPt.Y * vecPt.Y - 3 * vecPt.X * vecPt.X) * (1 - vecPt.Z * vecPt.Z)
                            + Math.Pow((vecPt.X * vecPt.X + vecPt.Y * vecPt.Y), 2)
                            - (9 * vecPt.Z * vecPt.Z - 1) * (1 - vecPt.Z * vecPt.Z) - m_fGap);
            }
        }

        public class ImplicitSuperEllipsoid : IImplicit
        {
            protected float     m_fAx;
            protected float     m_fAy;
            protected float     m_fAz;
            protected float     m_fEpsilon1;
            protected float     m_fEpsilon2;
            protected Vector3   m_vecCentre;

            /// <summary>
            /// Helper class for an implicit super ellipsoid.
            /// https://en.wikipedia.org/wiki/Superellipsoid
            /// </summary>
            public ImplicitSuperEllipsoid(Vector3 vecCentre, float fAx, float fAy, float fAz,  float fEpsilon1, float fEpsilon2)
            {
                m_fAx           = fAx;                
                m_fAy           = fAy;
                m_fAz           = fAz;
                m_fEpsilon1     = fEpsilon1;
                m_fEpsilon2     = fEpsilon2;
                m_vecCentre     = vecCentre;
            }

            public float fSignedDistance(in Vector3 vecPt)
            {
                double dX           = Math.Abs(vecPt.X + m_vecCentre.X)/ m_fAx;
                double dY           = Math.Abs(vecPt.Y + m_vecCentre.Y)/ m_fAy;
                double dZ           = Math.Abs(vecPt.Z + m_vecCentre.Z)/ m_fAz;

                double dDist        = Math.Pow((Math.Pow(dX, 2 / m_fEpsilon2) + Math.Pow(dY, 2 / m_fEpsilon2)), m_fEpsilon2 / m_fEpsilon1)
                                        + Math.Pow(dZ, 2 / m_fEpsilon1) ;
                float fFinalDist    = (float)(dDist - 1);
                return fFinalDist;
            }
        }
    }
}