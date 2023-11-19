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
        public class LatticePipe : BaseShape, ILatticeBaseShape, ISpineBaseShape
        {
            protected LineModulation m_oRadiusModulation;
            protected uint           m_nLengthSteps;
            protected Frames         m_aFrames;

            /// <summary>
            /// Initialises a round pipe from lattices based on a local frame and 2 dimensions.
            /// The shape has no spine.
            /// </summary>
            public LatticePipe(LocalFrame oFrame, float fLength, float fRadius = 10f) : base()
            {
                m_aFrames           = new Frames(fLength, oFrame);
                SetLengthSteps(100);

                m_oRadiusModulation = new LineModulation(fRadius);
                m_bTransformed      = false;
            }

            /// <summary>
            /// Initialises a round pipe from lattices based on a spine (frames) and 1 dimension.
            /// The spine replaces the length dimension.
            /// </summary>
            public LatticePipe(Frames aFrames, float fRadius = 10f) : base()
            {
                m_aFrames           = aFrames;
                SetLengthSteps(500);

                m_oRadiusModulation = new LineModulation(fRadius);
                m_bTransformed      = false;
            }


            //settings
            public void SetRadius(LineModulation oModulation)
            {
                m_oRadiusModulation = oModulation;
            }

            public void SetLengthSteps(uint nLengthSteps)
            {
                m_nLengthSteps = nLengthSteps;
            }


            //construction
            public override Voxels voxConstruct()
            {
                Lattice oLattice = latConstruct();
                Voxels oVoxels   = new Voxels(oLattice);
                return oVoxels;
            }

            public virtual Lattice latConstruct()
            {
                Lattice oLattice = new Lattice();
                for (int iZStep = 1; iZStep < m_nLengthSteps; iZStep++)
                {
                    float fLengthRatio0 = 1f / m_nLengthSteps * (iZStep - 1);
                    float fLengthRatio1 = 1f / m_nLengthSteps * (iZStep);

                    Vector3 vecPt0 = vecGetSpinePoint(fLengthRatio0);
                    Vector3 vecPt1 = vecGetSpinePoint(fLengthRatio1);

                    float fBeam0 = fGetRadius(fLengthRatio0);
                    float fBeam1 = fGetRadius(fLengthRatio1);

                    oLattice.AddSphere(vecPt0, fBeam0);
                }
                return oLattice;
            }

            /// <summary>
            /// Returns the centre axis position along the pipe. 
            /// </summary>
            public Vector3 vecGetSpinePoint(float fLengthRatio)
            {
                Vector3 vecPt = vecPt = m_aFrames.vecGetSpineAlongLength(fLengthRatio);
                if (m_bTransformed == true)
                {
                    vecPt = m_oTrafo(vecPt);
                }
                return vecPt;
            }

            protected float fGetRadius(float fLengthRatio)
            {
                float fRadius = m_oRadiusModulation.fGetModulation(fLengthRatio);
                return fRadius;
            }
        }
    }
}