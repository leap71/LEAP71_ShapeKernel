//
// SPDX-License-Identifier: CC0-1.0
//
// This example code file is released to the public under Creative Commons CC0.
// See https://creativecommons.org/publicdomain/zero/1.0/legalcode
//
// To the extent possible under law, LEAP 71 has waived all copyright and
// related or neighboring rights to this PicoGK example code file.
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

namespace PicoGKExamples
{
    ///////////////////////////////////////////////////////////////////////////
    // Below is a static class that implements a single static function
    // that can be called from Library::Go()

    class LatticeRobotExample
    {
        public static void Task()
        {
            var unitCellLocation = "LatticeRobot-Diamond_TPMS";
            var unitCell = new ImplicitUnitCell(Path.Combine(@"..\..\..\LatticeRobot_Library\", unitCellLocation), 2);

            // With this implementation of ImplicitUnitCell, we can only set constant parameters.  
            unitCell.SetParameter("gyroid", 0.25);


            try
            {
                Library.oViewer().SetGroupMaterial(0, "3291a0", 0f, 1f);

                // Create a new voxel field, which renders the lattice
                // we are passing the bounding box of the lattice, so that
                // we know which area in the voxel field to evaluate

                Voxels voxL = new(unitCell, unitCell.Bounds);

                // Let's show what we got
                Library.oViewer().Add(voxL);

            }

            catch (Exception e)
            {
                Library.Log($"Failed to run example: \n{e.Message}"); ;
            }
        }
    }
}

