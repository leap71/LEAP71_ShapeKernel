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
        public class LatticeManifold : LatticePipe
        {
            protected float m_fMaxPrintableRadius;
            protected float m_fLimitAngle;
            protected bool  m_bExtendBothSides;

            /// <summary>
            /// Initialises a manifold pipe from lattices based on a local frame and 2 dimensions.
            /// The overhang angle dictates the tear-drop shape of the pipe's cross-section.
            /// The tear-drop extension can be toggled for both directions (+z and -z).
            /// The shape has no spine.
            /// </summary>
            public LatticeManifold(
                LocalFrame  oFrame,
                float       fLength,
                float       fRadius             = 10f,
                float       fMaxOverhangAngle   = 45f,
                bool        bExtendBothSides    = false,
                float       fMinPrintableRadius = 0.1f
                ) : base(oFrame, fLength, fRadius)
            {
                m_fMaxPrintableRadius   = fMinPrintableRadius;
                m_fLimitAngle           = fMaxOverhangAngle;
                m_bExtendBothSides      = bExtendBothSides;
                SetLengthSteps(100);
            }

            /// <summary>
            /// Initialises a manifold pipe from lattices based on a spine (frames) and 1 dimension.
            /// The spine replaces the length dimension.
            /// The overhang angle dictates the tear-drop shape of the pipe's cross-section.
            /// The tear-drop extension can be toggled for both directions (+z and -z).
            /// </summary>
            public LatticeManifold(
                Frames      aFrames,
                float       fRadius             = 10f,
                float       fMaxOverhangAngle   = 45f,
                bool        bExtendBothSides    = false,
                float       fMinPrintableRadius = 0.1f
                ) : base(aFrames, fRadius)
            {
                m_fMaxPrintableRadius   = fMinPrintableRadius;
                m_fLimitAngle           = fMaxOverhangAngle;
                m_bExtendBothSides      = bExtendBothSides;
                SetLengthSteps(500);
            }

            protected float fGetRadius(float fLengthRatio, Vector3 vecDir)
            {
                float fRadius = fGetRadius(fLengthRatio);
                return fRadius;
            }

            public override Lattice latConstruct()
            {
                Lattice oLattice        = new Lattice();
                for (int iZStep = 0; iZStep < m_nLengthSteps; iZStep++)
                {
                    float fLengthRatio  = 1f / m_nLengthSteps * (iZStep);
                    Vector3 vecPt       = vecGetSpinePoint(fLengthRatio);
                    float fBeam         = fGetRadius(fLengthRatio);

                    //add round pipe point
                    oLattice.AddBeam(vecPt, fBeam, vecPt, fBeam);

                    //add tips
                    AddTip(ref oLattice, vecPt, fBeam, true);
                    if (m_bExtendBothSides == true)
                    {
                        AddTip(ref oLattice, vecPt, fBeam, false);
                    }
                }
                return oLattice;
            }

            protected void AddTip(ref Lattice oLattice, Vector3 vecPt, float fBeam, bool bZPositive = true)
            {
                //teardrop shape calculations:
                //https://de.wikipedia.org/wiki/Kreissegment

                float fHalfAlpha        = 90f - m_fLimitAngle;
                float fR                = fBeam;
                float fH                = fR * (1 - MathF.Cos(fHalfAlpha / 180f * MathF.PI));
                float fS                = 2f * fR * MathF.Sin(fHalfAlpha / 180f * MathF.PI);
                float fTipLength        = MathF.Tan(fHalfAlpha / 180f * MathF.PI) * (0.5f * fS - m_fMaxPrintableRadius);

                if (bZPositive == true)
                {
                    Vector3 vecMidSehne = vecPt + (fR - fH) * Vector3.UnitZ;
                    Vector3 vecTip      = vecMidSehne + fTipLength * Vector3.UnitZ;
                    oLattice.AddBeam(vecMidSehne, 0.5f * fS, vecTip, m_fMaxPrintableRadius, false);
                }
                else
                {
                    Vector3 vecMidSehne = vecPt - (fR - fH) * Vector3.UnitZ;
                    Vector3 vecTip      = vecMidSehne - fTipLength * Vector3.UnitZ;
                    oLattice.AddBeam(vecMidSehne, 0.5f * fS, vecTip, m_fMaxPrintableRadius, false);
                }
            }

            protected static float fGetOverhangAngleDegFromVector(Vector3 vecDir)
            {
                vecDir                  = vecDir.Normalize();
                float fConnectionAngle  = VecOperations.fGetAngleBetween(vecDir, -Vector3.UnitZ);
                float fDegAngle         = fConnectionAngle / MathF.PI * 180f;
                return fDegAngle;
            }
        }
    }
}