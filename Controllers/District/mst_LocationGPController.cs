using APATools.Context;
using APATools.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace APATools.Controllers.District
{
    public class mst_LocationGPController : Controller
    {
        private readonly APAToolsContext _context;

        public mst_LocationGPController(APAToolsContext context)
        {
            _context = context;
        }

        // GET: mst_LocationGP
        public async Task<IActionResult> Index()
        {
            var aPAToolsContext = _context.mst_LocationGPs.Include(m => m.BlockCodeNavigation);
            return View(await aPAToolsContext.ToListAsync());
        }

        // GET: mst_LocationGP/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mst_LocationGP = await _context.mst_LocationGPs
                .Include(m => m.BlockCodeNavigation)
                .FirstOrDefaultAsync(m => m.GPCode == id);
            if (mst_LocationGP == null)
            {
                return NotFound();
            }

            return View(mst_LocationGP);
        }

        // GET: mst_LocationGP/Create
        public IActionResult Create()
        {
            ViewData["BlockCode"] = new SelectList(_context.mst_LocationBlocks, "BlockCode", "BlockCodeGIS");
            return View();
        }

        // POST: mst_LocationGP/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,GPCode,GPName,GPCodeGPMS,GPCodeOldMIS,GPCodeGIS,BlockCode,GPCodeNEWGPMS,ActiveStatus,DeleteStatus,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn")] mst_LocationGP mst_LocationGP)
        {
            if (ModelState.IsValid)
            {
                _context.Add(mst_LocationGP);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BlockCode"] = new SelectList(_context.mst_LocationBlocks, "BlockCode", "BlockCodeGIS", mst_LocationGP.BlockCode);
            return View(mst_LocationGP);
        }

        // GET: mst_LocationGP/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mst_LocationGP = await _context.mst_LocationGPs.FindAsync(id);
            if (mst_LocationGP == null)
            {
                return NotFound();
            }
            ViewData["BlockCode"] = new SelectList(_context.mst_LocationBlocks, "BlockCode", "BlockCodeGIS", mst_LocationGP.BlockCode);
            return View(mst_LocationGP);
        }

        // POST: mst_LocationGP/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("id,GPCode,GPName,GPCodeGPMS,GPCodeOldMIS,GPCodeGIS,BlockCode,GPCodeNEWGPMS,ActiveStatus,DeleteStatus,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn")] mst_LocationGP mst_LocationGP)
        {
            if (id != mst_LocationGP.GPCode)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(mst_LocationGP);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!mst_LocationGPExists(mst_LocationGP.GPCode))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["BlockCode"] = new SelectList(_context.mst_LocationBlocks, "BlockCode", "BlockCodeGIS", mst_LocationGP.BlockCode);
            return View(mst_LocationGP);
        }

        // GET: mst_LocationGP/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mst_LocationGP = await _context.mst_LocationGPs
                .Include(m => m.BlockCodeNavigation)
                .FirstOrDefaultAsync(m => m.GPCode == id);
            if (mst_LocationGP == null)
            {
                return NotFound();
            }

            return View(mst_LocationGP);
        }

        // POST: mst_LocationGP/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var mst_LocationGP = await _context.mst_LocationGPs.FindAsync(id);
            if (mst_LocationGP != null)
            {
                _context.mst_LocationGPs.Remove(mst_LocationGP);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool mst_LocationGPExists(long id)
        {
            return _context.mst_LocationGPs.Any(e => e.GPCode == id);
        }
    }
}
