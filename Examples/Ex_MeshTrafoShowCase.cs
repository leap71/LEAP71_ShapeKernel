//
// SPDX-License-Identifier: CC0-1.0
//
// This example code file is released to the public under Creative Commons CC0.
// See https://creativecommons.org/publicdomain/zero/1.0/legalcode
//
// To the extent possible under law, LEAP 71 has waived all copyright and
// related or neighboring rights to this PicoGK Example Code.
//
// THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS
// OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//


using PicoGK;
using System.Numerics;


namespace Leap71
{
    using ShapeKernel;

    namespace ShapeKernelExamples
    {
        class MeshTrafoShowCase
        {
            public static void Task()
            {
                //Step 1: Generate a simple mesh (e.g. from voxelfields)
                BaseBox oBox                = new BaseBox(new LocalFrame(new Vector3(0, 100, 0)), 50, 40, 30);
                Voxels voxBox               = oBox.voxConstruct();
                Mesh mshBox                 = new Mesh(voxBox);


                //Step 2: Apply a vertex-wise transformation to the mesh
                // Select between the fnMirror and the fnRotate function
                //Mesh mshTrafoBox            = MeshUtility.mshApplyTransformation(mshBox, fnMirror);
                Mesh mshTrafoBox            = MeshUtility.mshApplyTransformation(mshBox, fnRotate);


                //Step 3: Voxelize transformed mesh (if needed)
                Voxels voxTrafoBox          = new Voxels(mshTrafoBox);
                Sh.PreviewVoxels(voxBox,      Cp.clrOrchid, 0.5f);
                Sh.PreviewVoxels(voxTrafoBox, Cp.clrOrchid, 1.0f);
            }

            /// <summary>
            /// Applies a transformation to the input vertex coordinates.
            /// Returns a new transformed point.
            /// This function mirrors the y-coordiante on the xz-plane.
            /// </summary>
            public static Vector3 fnMirror(Vector3 vecPt)
            {
                float fX            = vecPt.X;
                float fY            = vecPt.Y;
                float fZ            = vecPt.Z;
                float fMirrorPlaneY = 0;
                float fNewY         = fMirrorPlaneY - (fY - fMirrorPlaneY);
                Vector3 vecNewPt    = new Vector3(fX, fNewY, fZ);
                return vecNewPt;
            }

            /// <summary>
            /// Applies a transformation to the input vertex coordinates.
            /// Returns a new transformed point.
            /// This function rotates the point 45 deg around the z-axis.
            /// </summary>
            public static Vector3 fnRotate(Vector3 vecPt)
            {
                Vector3 vecAxis     = Vector3.UnitZ;
                float dPhi          = 45f / 180f * MathF.PI;
                Vector3 vecNewPt    = VecOperations.vecRotateAroundAxis(vecPt, dPhi, vecAxis);
                return vecNewPt;
            }
        }
    }
}