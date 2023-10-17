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
        public static class VecOperations
        {
            /// <summary>
            /// Sets the vector's length to 1.
            /// </summary>
            public static Vector3 Normalize(this Vector3 vecA)
            {
                float fLength = vecA.Length();
                if (fLength > 0.000000001f)
                {
                    Vector3 vecNorm = vecA / fLength;
                    return vecNorm;
                }
                return new Vector3(0f, 0f, 0f);
            }

            /// <summary>
            /// Adds z-dimension the flat vector.
            /// </summary>
            public static Vector3 ConvertTo3D(this Vector2 vecFlat, float fZ = 0)
            {
                return new Vector3(vecFlat.X, vecFlat.Y, fZ);
            }

            /// <summary>
            /// Removes z-dimension from vector.
            /// The information about the z-value will be lost.
            /// </summary>
            public static Vector2 ConvertTo2D(this Vector3 vecA)
            {
                return new Vector2(vecA.X, vecA.Y);
            }

            /// <summary>
            /// Returns the carthesian coordinates of a point specified in cylindrical coordinates.
            /// The angle is measured in radiant.
            /// </summary>
            public static Vector3 vecGetCylPoint(float fRadius, float fPhi, float fZ)
            {
                float fX        = fRadius * MathF.Cos(fPhi);
                float fY        = fRadius * MathF.Sin(fPhi);
                Vector3 vecPt   = new Vector3(fX, fY, fZ);
                return vecPt;
            }

            /// <summary>
            /// Returns the carthesian coordinates of a point specified in spherical coordinates.
            /// All angles are measured in radiant.
            /// </summary>
            public static Vector3 vecGetSphPoint(float fRadius, float fPhi, float fTheta)
            {
                float fX        = fRadius * MathF.Cos(fPhi) * MathF.Cos(fTheta);
                float fY        = fRadius * MathF.Sin(fPhi) * MathF.Cos(fTheta);
                float fZ        = fRadius * MathF.Sin(fTheta);
                Vector3 vecPt   = new Vector3(fX, fY, fZ);
                return vecPt;
            }

            /// <summary>
            /// Returns the 2D planar radius of a point with respect to the absolute z-axis.
            /// </summary>
            public static float fGetRadius(Vector3 vecPt)
            {
                float fRadius = MathF.Sqrt(vecPt.X * vecPt.X + vecPt.Y * vecPt.Y);
                return fRadius;
            }

            /// <summary>
            /// Returns the 2D planar polar angle of a point with respect to the absolute z-axis.
            /// Cylindrical coordinate system.
            /// The angle is measured in radiant.
            /// </summary>
            public static float fGetPhi(Vector3 vecPt)
            {
                float fPhi = MathF.Atan2(vecPt.Y, vecPt.X);
                return fPhi;
            }

            /// <summary>
            /// Returns the azimuthal angle of a point with respect to the absolute z-axis.
            /// Spherical coordinate system.
            /// The angle is measured in radiant.
            /// </summary>
            public static float fGetTheta(Vector3 vecPt)
            {
                float fRadius   = fGetRadius(vecPt);
                float fTheta    = MathF.Atan2(vecPt.Z, fRadius);
                return fTheta;
            }

            /// <summary>
            /// Returns a point with the same cylindrical coordinates (z and phi), but with a new radial position.
            /// </summary>
            public static Vector3 vecSetRadius(Vector3 vecPt, float fNewRadius)
            {
                float fZ            = vecPt.Z;
                float fRadius       = fGetRadius(vecPt);
                float fPhi          = fGetPhi(vecPt);
                Vector3 vecNewPt    = vecGetCylPoint(fNewRadius, fPhi, fZ);
                return vecNewPt;
            }

            /// <summary>
            /// Returns a point with the same cylindrical coordinates (z and radius), but with a new polar position.
            /// The angle is measured in radiant.
            /// </summary>
            public static Vector3 vecSetPhi(Vector3 vecPt, float fNewPhi)
            {
                float fZ            = vecPt.Z;
                float fRadius       = fGetRadius(vecPt);
                float fPhi          = fGetPhi(vecPt);
                Vector3 vecNewPt    = vecGetCylPoint(fRadius, fNewPhi, fZ);
                return vecNewPt;
            }

            /// <summary>
            /// Returns a point with the same coordinates, but with a new height position.
            /// </summary>
            public static Vector3 vecSetZ(Vector3 vecPt, float fNewZ)
            {
                float fRadius       = fGetRadius(vecPt);
                float fPhi          = fGetPhi(vecPt);
                Vector3 vecNewPt    = vecGetCylPoint(fRadius, fPhi, fNewZ);
                return vecNewPt;
            }

            /// <summary>
            /// Returns a point that is radially shifted by dRadius.
            /// </summary>
            public static Vector3 vecUpdateRadius(Vector3 vecPt, float dRadius)
            {
                float fZ            = vecPt.Z;
                float fNewRadius    = fGetRadius(vecPt) + dRadius;
                float fPhi          = fGetPhi(vecPt);
                Vector3 vecNewPt    = vecGetCylPoint(fNewRadius, fPhi, fZ);
                return vecNewPt;
            }

            /// <summary>
            /// Returns a point that is turned around the absolute z-axis by an angle dPhi.
            /// The angle increment is measured in radiant.
            /// </summary>
            public static Vector3 vecUpdatePhi(Vector3 vecPt, float dPhi)
            {
                float fZ            = vecPt.Z;
                float fRadius       = fGetRadius(vecPt);
                float fNewPhi       = fGetPhi(vecPt) + dPhi;
                Vector3 vecNewPt    = vecGetCylPoint(fRadius, fNewPhi, fZ);
                return vecNewPt;
            }

            /// <summary>
            /// Returns a point that is vertically shifted by dZ.
            /// </summary>
            public static Vector3 vecUpdateZ(Vector3 vecPt, float dZ)
            {
                float fRadius       = fGetRadius(vecPt);
                float fPhi          = fGetPhi(vecPt);
                float fNewZ         = vecPt.Z + dZ;
                Vector3 vecNewPt    = vecGetCylPoint(fRadius, fPhi, fNewZ);
                return vecNewPt;
            }

            /// <summary>
            /// Returns the normalized, 2D planar radial direction from the absolute z-axis to the specified point.
            /// </summary>
            public static Vector3 vecGetPlanarDir(Vector3 vecPt)
            {
                Vector3 vecDir  = new Vector3(vecPt.X, vecPt.Y, 0);
                vecDir          = vecDir.Normalize();
                return vecDir;
            }

            /// <summary>
            /// Returns the same or the flipped vector depending on which option is more aligned with the target direction.
            /// Two vectors are aligned when they point in the same direction.
            /// Two vectors can either be aligned, orthogonal to each other or not aligned.
            /// </summary>
            public static Vector3 vecFlipForAlignment(Vector3 vecDir, Vector3 vecTargetDir)
            {
                if (Vector3.Dot(vecTargetDir, vecDir) >= Vector3.Dot(vecTargetDir, -vecDir))
                {
                    return vecDir;
                }
                else
                {
                    return -vecDir;
                }
            }

            /// <summary>
            /// Returns true if the specified direction is aligned with the target direction.
            /// Two vectors are aligned when they point in the same direction.
            /// Two vectors can either be aligned, orthogonal to each other or not aligned.
            /// </summary>
            public static bool bCheckAlignment(Vector3 vecDir, Vector3 vecTargetDir)
            {
                if (Vector3.Dot(vecTargetDir, vecDir) >= Vector3.Dot(vecTargetDir, -vecDir))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            /// <summary>
            /// Rotates a point around the absolute z-axis.
            /// The axis origin can be customised.
            /// The angle increment is measured in radiant.
            /// </summary>
            public static Vector3 vecRotateAroundZ(Vector3 vecPt, float dPhi, Vector3 vecAxisOrigin = new Vector3())
            {
                Vector3 vecDiff     = vecPt - vecAxisOrigin;
                float fPhi          = fGetPhi(vecDiff);
                Vector3 vecRotDiff  = vecSetPhi(vecDiff, fPhi + dPhi);
                Vector3 vecRotPt    = vecAxisOrigin + vecRotDiff;
                return vecRotPt;
            }

            /// <summary>
            /// Returns a arbitary vector that is orthogonal to the specified direction.
            /// </summary>
            public static Vector3 vecGetOrthogonalDir(Vector3 vecDir)
            {
                Vector3 vecNonParallel = Vector3.UnitX;
                if (MathF.Abs(Vector3.Dot(vecDir, vecNonParallel)) > 0.95f)
                {
                    vecNonParallel  = Vector3.UnitY;
                }
                Vector3 vecNormal   = Vector3.Cross(vecDir, vecNonParallel);
                vecNormal           = vecNormal.Normalize();
                return vecNormal;
            }

            /// <summary>
            /// Returns the minimum angle between to 3D vectors.
            /// The angle is measured in radiant.
            /// </summary>
            public static float fGetAngleBetween(Vector3 vecA, Vector3 vecB)
            {
                float fNormA = vecA.Length();
                float fNormB = vecB.Length();
                vecA        /= fNormA;
                vecB        /= fNormB;
                float fDot   = Uf.fLimitValue(Vector3.Dot(vecA, vecB), -1, 1);
                float fTheta = MathF.Acos(fDot);

                if (float.IsNaN(fTheta))
                {
                    Library.Log("Invalid rotation angle.");
                    throw new Exception("Invalid rotation angle.");
                }
                return fTheta;
            }

            /// <summary>
            /// Returns the minimum, signed angle between to 3D vectors.
            /// The angle is measured in radiant.
            /// The order in which the vectors are specified will influende the rotation sense indicated by the sign.
            /// </summary>
            public static float fGetSignedAngleBetween(Vector3 vecA, Vector3 vecB)
            {
                float fNormA = vecA.Length();
                float fNormB = vecB.Length();
                vecA        /= fNormA;
                vecB        /= fNormB;
                float fDot   = Uf.fLimitValue(Vector3.Dot(vecA, vecB), -1, 1);
                float fTheta = MathF.Acos(fDot);

                if (float.IsNaN(fTheta))
                {
                    Library.Log("Invalid rotation angle.");
                    throw new Exception("Invalid rotation angle.");
                }

                Vector3 vecAxis     = Vector3.Cross(vecA, vecB);
                Vector3 vecPosRotB  = vecRotateAroundAxis(vecB, fTheta, vecAxis);
                float fPosDist      = (vecPosRotB - vecA).LengthSquared();

                Vector3 vecNegRotB  = vecRotateAroundAxis(vecB, -fTheta, vecAxis);
                float fNegDist      = (vecNegRotB - vecA).LengthSquared();

                if (fNegDist < fPosDist)
                {
                    return -fTheta;
                }
                else
                {
                    return fTheta;
                }
            }

            /// <summary>
            /// Rotates a point around a custom axis.
            /// The axis origin can be customised.
            /// The angle increment is measured in radiant.
            /// </summary>
            public static Vector3 vecRotateAroundAxis(Vector3 vecPt, float dPhi, Vector3 vecAxis, Vector3 vecAxisOrigin = new Vector3())
            {
                vecAxis                 = vecAxis.Normalize();
                LocalFrame oAxisFrame   = new LocalFrame(vecAxisOrigin, vecAxis);
                Vector3 vecLocalX       = oAxisFrame.vecGetLocalX();
                Vector3 vecLocalY       = oAxisFrame.vecGetLocalY();
                Vector3 vecLocalZ       = oAxisFrame.vecGetLocalZ();

                vecPt -= vecAxisOrigin;

                float fX = Vector3.Dot(vecPt, vecLocalX);
                float fY = Vector3.Dot(vecPt, vecLocalY);
                float fZ = Vector3.Dot(vecPt, vecLocalZ);

                float fRadius   = MathF.Sqrt(fX * fX + fY * fY);
                float fPhi      = MathF.Atan2(fY, fX);

                float fNewX     = fRadius * MathF.Cos(fPhi + dPhi);
                float fNewY     = fRadius * MathF.Sin(fPhi + dPhi);

                Vector3 vecNewPt =
                    vecAxisOrigin +
                    fNewX * vecLocalX +
                    fNewY * vecLocalY +
                    fZ * vecLocalZ;
                return vecNewPt;
            }

            /// <summary>
            /// Rotates and translates a point in absolute carthesian coordinates onto the specified local reference frame.
            /// The relative coordinates of the result with respect to the local frame will mach those of the input point in absolute reference.
            /// The absolute coordinated of the result will update depending on the position and orientation of the local frame.
            /// "How would this point look like when it was referenced to a this local frame?".
            /// </summary>
            public static Vector3 vecTranslatePointOntoFrame(LocalFrame oFrame, Vector3 vecPt)
            {
                Vector3 vecOrigin = oFrame.vecGetPosition();
                Vector3 vecNewPt =
                        vecOrigin +
                        vecPt.X * oFrame.vecGetLocalX() +
                        vecPt.Y * oFrame.vecGetLocalY() +
                        vecPt.Z * oFrame.vecGetLocalZ();
                return vecNewPt;
            }

            /// <summary>
            /// Returns the relative coordinates expression of an absolute, carthesian point with respect to a given local reference frame.
            /// "How would this point look like when viewed from this local frame?".
            /// </summary>
            public static Vector3 vecExpressPointInFrame(LocalFrame oFrame, Vector3 vecPt)
            {
                vecPt   -= oFrame.vecGetPosition();
                float fX = Vector3.Dot(vecPt, oFrame.vecGetLocalX());
                float fY = Vector3.Dot(vecPt, oFrame.vecGetLocalY());
                float fZ = Vector3.Dot(vecPt, oFrame.vecGetLocalZ());
                Vector3 vecNewPt = new Vector3(fX, fY, fZ);
                return vecNewPt;
            }

            /// <summary>
            /// Returns the direction of a direct connection between the local frame's z-axis and the specified point.
            /// Similar to the function vecGetPlanarDir, but in an arbitary, 3D reference frame.
            /// </summary>
            public static Vector3 vecGetDirectionToAxis(LocalFrame oFrame, Vector3 vecPt)
            {
                Vector3 vecLocalX = oFrame.vecGetLocalX();
                Vector3 vecLocalY = oFrame.vecGetLocalY();
                Vector3 vecLocalZ = oFrame.vecGetLocalZ();

                vecPt   -= oFrame.vecGetPosition();
                float fX = Vector3.Dot(vecPt, vecLocalX);
                float fY = Vector3.Dot(vecPt, vecLocalY);

                Vector3 vecNewPt =
                    fX * vecLocalX +
                    fY * vecLocalY;

                vecNewPt = vecNewPt.Normalize();
                return vecNewPt;
            }

            /// <summary>
            /// Returns the radius between the local frame's z-axis and the specified point.
            /// Similar to the function fGetRadius, but in an arbitary, 3D reference frame.
            /// </summary>
            public static float fGetRadiusToAxis(LocalFrame oFrame, Vector3 vecPt)
            {
                Vector3 vecLocalX = oFrame.vecGetLocalX();
                Vector3 vecLocalY = oFrame.vecGetLocalY();
                Vector3 vecLocalZ = oFrame.vecGetLocalZ();

                vecPt   -= oFrame.vecGetPosition();
                float fX = Vector3.Dot(vecPt, vecLocalX);
                float fY = Vector3.Dot(vecPt, vecLocalY);

                float fRadius = MathF.Sqrt(fX * fX + fY * fY);
                return fRadius;
            }

            /// <summary>
            /// Returns the polar angle between the local frame's z-axis and the specified point.
            /// Similar to the function fGetPhi, but in an arbitary, 3D reference frame.
            /// The angle is measured in radiant.
            /// </summary>
            public static float fGetPhiToAxis(LocalFrame oFrame, Vector3 vecPt)
            {
                Vector3 vecLocalX = oFrame.vecGetLocalX();
                Vector3 vecLocalY = oFrame.vecGetLocalY();
                Vector3 vecLocalZ = oFrame.vecGetLocalZ();

                vecPt   -= oFrame.vecGetPosition();
                float fX = Vector3.Dot(vecPt, vecLocalX);
                float fY = Vector3.Dot(vecPt, vecLocalY);

                float fPhi = MathF.Atan2(fY, fX);
                return fPhi;
            }

            /// <summary>
            /// Returns a linearly interpolated point between the two specified points.
            /// </summary>
            public static Vector3 vecLinearInterpolation(Vector3 vecPt1, Vector3 vecPt2, float fRatio)
            {
                Vector3 vecInter = vecPt1 + fRatio * (vecPt2 - vecPt1);
                return vecInter;
            }

            /// <summary>
            /// Returns a cylidrically interpolated point between the two specified points.
            /// </summary>
            public static Vector3 vecCylindricalInterpolation(Vector3 vecPt1, Vector3 vecPt2, float fRatio, Vector3 vecAxisOrigin = new Vector3())
            {
                vecAxisOrigin       = vecSetZ(vecAxisOrigin, 0f);
                float dMinAngle     = fGetAngleBetween(vecPt1, vecPt2);

                Vector3 vecSide1    = (vecPt1 - vecAxisOrigin).Normalize();
                Vector3 vecSide2    = (vecPt2 - vecAxisOrigin).Normalize();
                Vector3 vecNormal   = Vector3.Cross(vecSide1, vecSide2);

                //figure out rotation sense
                float fDistPos      = (vecPt2 - vecRotateAroundZ(vecPt1, dMinAngle)).Length();
                float fDistNeg      = (vecPt2 - vecRotateAroundZ(vecPt1, -dMinAngle)).Length();

                int iSense = 1;
                if (fDistNeg < fDistPos)
                {
                    iSense = -1;
                }

                float fRadius1      = fGetRadius(vecPt1 - vecAxisOrigin);
                float fRadius2      = fGetRadius(vecPt2 - vecAxisOrigin);
                float fInterRadius  = fRadius1 + fRatio * (fRadius2 - fRadius1);

                float fInterZ       = vecPt1.Z + fRatio * (vecPt2.Z - vecPt1.Z);

                Vector3 vecInter    = vecRotateAroundZ(fInterRadius * vecSide1, iSense * fRatio * dMinAngle);
                vecInter            = vecSetZ(vecInter, fInterZ);
                return vecInter + vecAxisOrigin;
            }

            /// <summary>
            /// Returns a spherically interpolated point between the two specified points.
            /// </summary>
            public static Vector3 vecSphericalInterpolation(Vector3 vecPt1, Vector3 vecPt2, float fRatio, Vector3 vecAxisOrigin = new Vector3())
            {
                float dMinAngle     = fGetAngleBetween(vecPt1, vecPt2);

                Vector3 vecSide1    = (vecPt1 - vecAxisOrigin).Normalize();
                Vector3 vecSide2    = (vecPt2 - vecAxisOrigin).Normalize();
                Vector3 vecNormal   = Vector3.Cross(vecSide1, vecSide2);

                //figure out rotation sense
                float fDistPos      = (vecPt2 - vecRotateAroundAxis(vecPt1, dMinAngle, vecNormal)).Length();
                float fDistNeg      = (vecPt2 - vecRotateAroundAxis(vecPt1, -dMinAngle, vecNormal)).Length();

                int iSense = 1;
                if (fDistNeg < fDistPos)
                {
                    iSense = -1;
                }

                float fRadius1      = (vecPt1 - vecAxisOrigin).Length();
                float fRadius2      = (vecPt2 - vecAxisOrigin).Length();
                float fInterRadius  = fRadius1 + fRatio * (fRadius2 - fRadius1);

                Vector3 vecInter    = vecRotateAroundAxis(fInterRadius * vecSide1, iSense * fRatio * dMinAngle, vecNormal);
                return vecInter + vecAxisOrigin;
            }
        }
    }
}