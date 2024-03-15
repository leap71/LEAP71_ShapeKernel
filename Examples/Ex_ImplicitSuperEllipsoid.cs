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
        class ImplicitSolidEllipsoid
        {
            public static void Task()
            {
                //note: ellipsoid shape is very small. Use voxel size 0.01mm for good resolution.
                //note: https://en.wikipedia.org/wiki/Superellipsoid
                //note: parameter n is fEpsilon1
                //note: parameter e is fEpsilon2
                try
                {
                    {                    
                        float fAx                   = 1f;
                        float fAy                   = 1f;
                        float fAz                   = 1f;
                        float fEpsilon1             = 3.00f;
                        float fEpsilon2             = 0.25f;
                        Vector3 vecCentre           = new Vector3(0f, 0, 0);


                        //Step 1: generate SDF
                        IImplicit sdfEllipsoid      = new ImplicitSuperEllipsoid(vecCentre, fAx, fAz, fAy, fEpsilon1, fEpsilon2);

                        //Step 2: define bounding object
                        BBox3 oBBox                 = new BBox3(1f * new Vector3(-fAx - vecCentre.X, -fAy - vecCentre.Y, -fAz - vecCentre.Z),
                                                                1f * new Vector3(fAx - vecCentre.X, fAy - vecCentre.Y, fAz - vecCentre.Z));

                        //Step 3: render the implicit shape into voxels
                        Voxels voxEllipsoid         = new Voxels(sdfEllipsoid, oBBox);

                        //Step 4: visualization
                        Sh.PreviewVoxels(voxEllipsoid, Cp.clrRuby);
                    }

                    {                        
                        float fAx                   = 1f;
                        float fAy                   = 1f;
                        float fAz                   = 1f;
                        float fEpsilon1             = 1.50f;
                        float fEpsilon2             = 1.50f;
                        Vector3 vecCentre           = new Vector3(-4, 0, 0);

                        //Step 1: generate SDF
                        IImplicit sdfEllipsoid      = new ImplicitSuperEllipsoid(vecCentre, fAx, fAz, fAy, fEpsilon1, fEpsilon2);

                        //Step 2: define bounding object
                        BBox3 oBBox                 = new BBox3(1f * new Vector3(-fAx - vecCentre.X, -fAy - vecCentre.Y, -fAz - vecCentre.Z),
                                                                1f * new Vector3( fAx - vecCentre.X,  fAy - vecCentre.Y,  fAz - vecCentre.Z));

                        //Step 3: render the implicit shape into voxels
                        Voxels voxEllipsoid         = new Voxels(sdfEllipsoid, oBBox);

                        //Step 4: visualization
                        Sh.PreviewVoxels(voxEllipsoid, Cp.clrBlue);
                    }

                    {
                        float fAx                   = 1f;
                        float fAy                   = 1f;
                        float fAz                   = 1f;
                        float fEpsilon1             = 0.25f;
                        float fEpsilon2             = 0.25f;
                        Vector3 vecCentre           = new Vector3(4, 0, 0);

                        //Step 1: generate SDF
                        IImplicit sdfEllipsoid      = new ImplicitSuperEllipsoid(vecCentre, fAx, fAy, fAz, fEpsilon1, fEpsilon2);

                        //Step 2: define bounding object
                        BBox3 oBBox                 = new BBox3(1f * new Vector3(-fAx - vecCentre.X, -fAy - vecCentre.Y, -fAz - vecCentre.Z),
                                                                1f * new Vector3(fAx - vecCentre.X, fAy - vecCentre.Y, fAz - vecCentre.Z));

                        //Step 3: render the implicit shape into voxels
                        Voxels voxEllipsoid         = new Voxels(sdfEllipsoid, oBBox);

                        //Step 4: visualization
                        Sh.PreviewVoxels(voxEllipsoid, Cp.clrBubblegum);
                    }
                }
                catch (Exception e)
                {
                    Library.Log($"Failed run example: \n{e.Message}"); ;
                }
            }
        }
    }
}