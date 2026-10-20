# Mcd.MatVal storage, packing and copy constructor members

- **Original:** mplrun.dll 12.0.1.0, SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`.
- **Reference:** ilspycmd 11.1.0.9782 (`tools/reference/reference.py`); inspection record `ev_c8ca49d8286456aa09639a00185113080927a4662dda8cb0c122b48acc2f6af8`, managed-member inspection.
- **Quire:** `src/Quire.Mpl/Mcd/MatVal.Storage.cs`; tests: `MatValStorageDifferentialTests`.
- Contracts recorded before implementation. Units conversion is a representation cast, not physical scaling. Original unexpected mutations and indexing are intentional fidelity requirements.

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| Mcd.MatVal.Copy(r8[], i4, r8[], i4, i4) | 0x06000534 | If count > 0, pin destination array at dst_offset and copy count doubles from src at src_offset via Marshal.Copy. Count <= 0 is a no-op. | Direct case: positive count and nonpositive count boundaries. Exact Release IL match. | Verified |
| Mcd.MatVal.Pack(r8[]) | 0x06000535 | Allocate double[Rows * Cols]. If Anr == Rows, Copy entire block; otherwise copy row by row from Index(0, i) with stride Anr. | Direct case: packed and unpacked array inputs. Exact Release IL match. | Verified |
| Mcd.MatVal.Pack(r8[], i4, i4, i4, i4) | 0x06000536 | Allocate double[(r2-r1) * (c2-c1)]. Copy column by column from Index(r1, i) with stride Anr. | Direct case: submatrix bounds. Exact Release IL match. | Verified |
| Mcd.MatVal.Pack(r8[], i4) | 0x06000537 | Allocate double[Rows]. Copy single column from Index(0, c). | Direct case: column index. Exact Release IL match. | Verified |
| Mcd.MatVal.PackZero(r8[]) | 0x06000541 | Scan array; return original array if any element != 0.0; return null if all elements are 0.0. | Direct cases: all zero, nonzero elements. Exact Release IL match. | Verified |
| Mcd.MatVal.PackReal() | 0x06000542 | Return Pack(m_rarr). | Direct cases: packed and unpacked matrices. Exact Release IL match. | Verified |
| Mcd.MatVal.PackReal(i4) | 0x06000543 | Return Pack(m_rarr, c). | Direct cases: columns 0 and 1. Exact Release IL match. | Verified |
| Mcd.MatVal.PackReal(i4, i4, i4, i4) | 0x06000544 | Return Pack(m_rarr, r1, r2, c1, c2). | Direct case: submatrix bounds. Exact Release IL match. | Verified |
| Mcd.MatVal.PackImag() | 0x06000545 | If m_iarr != null, return PackZero(Pack(m_iarr)); otherwise null. | Direct cases: absent and present imaginary backings. Exact Release IL match. | Verified |
| Mcd.MatVal.PackImag(i4) | 0x06000546 | If m_iarr != null, return PackZero(Pack(m_iarr, c)); otherwise null. | Direct cases: absent and present imaginary column. Exact Release IL match. | Verified |
| Mcd.MatVal.PackImag(i4, i4, i4, i4) | 0x06000547 | If m_iarr != null, return PackZero(Pack(m_iarr, r1, r2, c1, c2)); otherwise null. | Direct cases: absent and present imaginary submatrix. Exact Release IL match. | Verified |
| Mcd.MatVal.PackPtrs(i4) | 0x0600054a | Delegate to PackPtrs(0, Rows, c, c + 1). | Direct cases: column with and without pointers. Exact Release IL match. | Verified |
| Mcd.MatVal.PackPtrs(i4, i4, i4, i4) | 0x0600054b | If m_parr == null return null. Scan submatrix bounds; for each live pointer Nan, insert into new Hashtable shifted to (0,0); return table if any inserted, else null. | Direct cases: null table, present pointers in range, and pointers outside range. Exact Release IL match. | Verified |
| Mcd.MatVal..ctor(class:532) | 0x0600048d | Copy constructor: calls this(PackReal(), PackImag(), Rows, Cols, Rows, null, Flags.ComplexSnan, 0); copies dimension; if RepHasPtrs calls InitPtrs(m). | Direct cases: real matrix, imaginary matrix, and pointer matrix. Exact Release IL match. | Verified |
| Mcd.MatVal.InitPtrs(class:532) | 0x0600048e | If source is Packed and (Cols == 1 || Anr == m.Anr), clone source m_parr; otherwise new Hashtable and CopyPtrs(m). | Direct cases: packed matrix cloning and unpacked matrix translation. Exact Release IL match. | Verified |
| Mcd.MatVal.IteratePtrs(class:535) | 0x0600054c | If m_parr != null, enumerate DictionaryEntry; for live PtrNans, invoke callback with RowOf, ColOf, and entry.Value. | Direct cases: null table and present pointers. Exact Release IL match. | Verified |
| Mcd.MatVal.TransformPtrs(class:532, class:535) | 0x0600054d | If m.RepHasPtrs, ensure m_parr != null, call m.IteratePtrs(op); if m_parr.Count == 0 reset to null. | Direct cases: source without pointers, source with pointers, empty destination reset. Exact Release IL match. | Verified |
| Mcd.MatVal.CopyPtrs(class:532) | 0x06000555 | Call TransformPtrs(m, set_ptrs_2). | Direct cases: source with live pointers copied to receiver. Exact Release IL match. | Verified |

## Test cases

Direct comparisons in `MatValStorageDifferentialTests` invoke 35 direct cases across all 18 storage, packing, pointer enumeration, transformation and copy constructor members on both artifacts. Comparisons verify return values, exception behaviors, shape, stride, offset, flags, dimension, backing arrays and pointer tables across receivers and arguments. Conclusions are limited to the inputs and branches exercised in the current focused validation run.

## Structure mismatches observed in the focused run

None. All 18 methods in `Mcd.MatVal.Storage` achieve 100% exact Release IL matching against Mathcad Prime 12.0.1.0 `mplrun.dll`.
