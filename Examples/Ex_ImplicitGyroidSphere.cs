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
        class ImplicitGyroidSphere
        {
            public static void Task()
            {
                try
                {
                    //generate SDFs using presets.
                    Vector3 vecCentre       = new Vector3();
                    float fRadius           = 10f;
                    IImplicit sdfSphere     = new ImplicitSphere(vecCentre, fRadius);
                    IImplicit sdfPattern    = new ImplicitGyroid(3, 1);
                    BBox3 oBBox             = new BBox3(1.2f * new Vector3(-fRadius, -fRadius, -fRadius), 1.2f * new Vector3(fRadius, fRadius, fRadius));

                    //render a simple shape into voxels.
                    Voxels voxSphere        = new Voxels(sdfSphere, oBBox);

                    //intersect an implicit pattern with a bounding voxelfield.
                    Voxels voxGyroidSphere  = Sh.voxIntersectImplicit(voxSphere, sdfPattern);

                    Sh.PreviewVoxels(voxGyroidSphere,   Cp.clrBillie);
                    Sh.PreviewVoxels(voxSphere,         Cp.clrBubblegum, 0.3f);
                }
                catch (Exception e)
                {
                    Library.Log($"Failed run example: \n{e.Message}"); ;
                }
            }
        }
    }
}