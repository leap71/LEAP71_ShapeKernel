//
// SPDX-License-Identifier: CC0-1.0
//
// This example code file is released to the public under Creative Commons CC0.
// See https://creativecommons.org/publicdomain/zero/1.0/legalcode
//
// To the extent possible under law, LEAP 71 has waived all copyright and
// related or neighboring rights to this LEAP 71 ShapeKernel Example Code.
//
// THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS
// OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//


using System.Numerics;
using PicoGK;


namespace Leap71
{
    using ShapeKernel;

    namespace ShapeKernelExamples
    {
        class ImplicitGenusGyroid
        {
            public static void Task()
            {
                //note: genus shape is very small. Use voxel size 0.01mm for good resolution.
                float fGap    = 0.3f;
                float fExtent = 2.5f;

                //Step 1: generate SDF
                IImplicit sdfGenus      = new ImplicitGenus(fGap);

                //Step 2: define bounding object
                BBox3 oBBox             = new BBox3(1.2f * new Vector3(-fExtent, -fExtent, -fExtent + 1.5f), 1.2f * new Vector3(fExtent, fExtent, fExtent - 1.5f));

                //Step 3: render the implicit shape into voxels
                Voxels voxGenus         = new Voxels(sdfGenus, oBBox);

                //Step 4: intersect  implicit pattern with the voxel
                IImplicit sdfPattern    = new ImplicitGyroid(1, 0.5f);
                Voxels voxGyroidGenus   = Sh.voxIntersectImplicit(voxGenus, sdfPattern);

                //Step 5: visualization
                Sh.PreviewVoxels(voxGyroidGenus, Cp.clrRandom());
                Sh.PreviewVoxels(voxGenus, Cp.clrRandom(), 0.5f);

                ////Step 6: export
                //Sh.ExportVoxelsToSTLFile(voxGenus, Sh.strGetExportPath(Sh.EExport.STL, "Genus"));
            }
        }
    }
}
