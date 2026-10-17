# Mcd.MatDim — batch 2b foundations

- **Original:** mplrun.dll, Prime 12.0.1.0; SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`; MVID `17a0933b-4988-4c18-963d-563998321cc8`.
- **REA:** `ev_c8ca49d8286456aa09639a00185113080927a4662dda8cb0c122b48acc2f6af8` (inspect_managed_members, 2026-10-14).
- **Decompiled view:** ILSpy 11.1.0.9782, run under Linux .NET 10 against the same installed file; raw view remains in /tmp.
- **Quire:** src/Quire.Mpl/Mcd/MatDim.cs; behavior tests MatDimTests; direct scenarios MatDimDifferentialTests.
- Scope is the exact rows below. Other methods, including nested MatDim implementations, remain open.

| Member (metadata parameter types) | Token | Normalized IL SHA-256 | Contract | Confidence / limits | Status |
|---|---|---|---|---|---|
| .ctor(i4, i4) | 0x0600060b | f6a49d03afc853abf73af67c21832fd146f004c057c7a0641b3fb4f066d75e39 | Preserve supplied row/column counts in readonly fields without validation. | Static observed; execution cases below only | Tested |
| dim(i4, i4) | 0x0600060c | declaration only | Abstract dimension lookup declaration. | Static observed; execution cases below only | Abstract |
| matdim(i4, i4) | 0x0600060d | declaration only | Abstract matrix dimension lookup declaration. | Static observed; execution cases below only | Abstract |
| get_UniformDim() | 0x0600060e | ce2965e0f5bbb04ab5d1eb4e0cc84a9790f9d24a539a14fd7c2dbc5ba7a2867f | Base returns null. | Same-artifact IL; 2 direct cases; state paths limited as documented in Lane B checkpoint | Verified |
| get_Rows() | 0x0600060f | 9bb810747031ca248a85499080df9f080363ade32958fc335a13c8c8c5a0ceea | Return logical row count. | Static observed; execution cases below only | Tested |
| get_Cols() | 0x06000610 | dfc637111280f7dfbc3f7dbf2e2fc517836b87f1461e9b21f655f9a304139dd3 | Return logical column count. | Static observed; execution cases below only | Tested |
| get_RowDimensions() | 0x06000611 | ce2965e0f5bbb04ab5d1eb4e0cc84a9790f9d24a539a14fd7c2dbc5ba7a2867f | Base returns null. | Same-artifact IL; 2 direct cases; state paths limited as documented in Lane B checkpoint | Verified |
| get_ColDimensions() | 0x06000612 | ce2965e0f5bbb04ab5d1eb4e0cc84a9790f9d24a539a14fd7c2dbc5ba7a2867f | Base returns null. | Same-artifact IL; 2 direct cases; state paths limited as documented in Lane B checkpoint | Verified |
| RowDimensionArray(i4) | 0x06000613 | 79fac32bd8571ab3e947468dbf50c81a006949278927d098e4336c72e8811f3c | Allocate Cols entries and query dim(row, each column). | Same-artifact IL; 4 direct cases; state paths limited as documented in Lane B checkpoint | Verified |
| ColDimensionArray(i4) | 0x06000614 | a6300da228782e8b3798bfcb9d23cd31d3b55629bec028b24e3e7cf26ed3f3fd | Allocate Rows entries and query dim(each row, column). | Same-artifact IL; 4 direct cases; state paths limited as documented in Lane B checkpoint | Verified |
| get_IsLinear() | 0x06000619 | ce2965e0f5bbb04ab5d1eb4e0cc84a9790f9d24a539a14fd7c2dbc5ba7a2867f | Base returns null. | Same-artifact IL; 2 direct cases; state paths limited as documented in Lane B checkpoint | Verified |
| CMult(class:572, class:572) | 0x0600061d | ce2965e0f5bbb04ab5d1eb4e0cc84a9790f9d24a539a14fd7c2dbc5ba7a2867f | Unconditionally return null, even for null operands. | Static observed; direct cases below only | Verified |
| Uniform(class:568[]) | 0x06000629 | 189a327d1bc862a38fc8f8a0c5a1d3d4ae2d2deb0a454a3ce7fa7872f170cae6 | Return first element only if all entries are reference-identical; null input returns null; empty input throws. | Static observed; direct cases below only | Verified |
| Eq_b(class:568[], class:568[]) | 0x0600062a | 003ccabd3fff913f5b899518244687e34b74afc12b3e9188dfcdf165686d662d | Array overload compares lengths then Dim.Eq_b per pair; empty equal arrays true; null arrays throw. | Static observed; direct cases below only | Verified |
| IsScalarMultiple(class:568[], class:568[]) | 0x0600062b | a00af22ef82ac24a56d7257fc9730900d13b6bcbac6319e15b35715bf38c705d | Compute d2[0]/d1[0], require reference-identical quotients for remaining d1 entries; inconsistent returns null. Empty or too-short rhs throws; extra rhs ignored. | Static observed; direct cases below only | Verified |
| Mult(class:568[], class:568[]) | 0x0600062c | 2bf1b5d405399b4b40ef53d17f01814b335fc89bb4aaa38763c8f3a4eb78480c | Array pair computes first element product then requires reference-identical products through d1 length; inconsistent returns null, extra rhs ignored, empty/short rhs throws. Scalar overload maps Dim.Mult into a fresh array. | Static observed; direct cases below only | Verified |
| Mult(class:568, class:568[]) | 0x0600062d | fc3cc7326df2a63968a1ae442d0af0018fe82944b35e3334070e0b865e4b6eaa | Array pair computes first element product then requires reference-identical products through d1 length; inconsistent returns null, extra rhs ignored, empty/short rhs throws. Scalar overload maps Dim.Mult into a fresh array. | Static observed; direct cases below only | Verified |
| Invert(class:568[]) | 0x0600062e | 971e5728cd33f04e61e0bb5950cca53e5a18bf5e2c19486e338f6f91731f0398 | Allocate fresh array of Dim.Pwr(element, −1); empty input yields empty array. | Static observed; direct cases below only | Verified |
| Product(class:568, class:568[]) | 0x0600062f | 72cbd49035442b1826246d70ac9545b2b4b9b4540d1a3db8c148ee6cf0f3f01e | Fold Dim.Mult from scalar through array; empty returns scalar. | Static observed; direct cases below only | Verified |
| Push(class:568, class:568[]) | 0x06000630 | b00faf7f38063b4d17d9f945ef6c4a67b61bd927a52e73159530a1d1d46cba4b | Allocate new array with first dimension prepended; input is unchanged. | Static observed; direct cases below only | Verified |
| c_dim..ctor(class:568[], i4) | 0x060007d5 | 3ed8e46824c37c277efd830868b9b37158da6f3f688e0bc88d50f509a4c5c4ee | Initialize base rows, cols from c.Length; assign m_coldim to c. | Same-artifact IL; 4 direct cases; state paths limited as documented in Lane B checkpoint | Verified |
| c_dim.make(class:568[], i4) | 0x060007d6 | 936602215b8315df780417f85d815de32230a6876e5ad336e53ff7c72c277f1d | Construct and return new c_dim instance with supplied column array and row count. | Same-artifact IL; 4 direct cases; state paths limited as documented in Lane B checkpoint | Verified |
| c_dim.eq_b(class:572) | 0x060007dc | 219987b242c90329d2a289fbc1c8b7278516009ad009ecbdde0f3845519961ff | Dispatch equality to m2.eq2_b(this). | Same-artifact IL; 7 direct cases; state paths limited as documented in Lane B checkpoint | Verified |
| c_dim.eq2_b(class:574) | 0x060007dd | 1a882f6aa8d0535aacbb4952b2fffab6cee55f5b9d6507780ad42b3a3e9c0e80 | Compare column factor arrays via Eq_b(m_coldim, m1.m_coldim); ignore row geometry. | Same-artifact IL; 6 direct cases; state paths limited as documented in Lane B checkpoint | Verified |
| r_dim..ctor(class:568[], i4) | 0x060007c3 | 5d60a56b9ab2c338259a2f61867e4e387da38cb6ec2bd577d9177372b8720549 | Initialize base rows from r.Length, cols from c; assign m_rowdim to r. | Same-artifact IL; 4 direct cases; state paths limited as documented in Lane B checkpoint | Verified |
| r_dim.make(class:568[], i4) | 0x060007c4 | b8375eed03f58bc3c1a15dcc3dad1dd507a2224acdb8fd7523413eb1ad3faf80 | Construct and return new r_dim instance with supplied row array and column count. | Same-artifact IL; 4 direct cases; state paths limited as documented in Lane B checkpoint | Verified |
| r_dim.eq_b(class:572) | 0x060007ca | 5136ca7ec8ab545b25ddc94d7ac0ea83f137514de0df7057adc1a8b85ff9f759 | Dispatch equality to m2.eq2_b(this). | Same-artifact IL; 5 direct cases; state paths limited as documented in Lane B checkpoint | Verified |
| r_dim.eq2_b(class:573) | 0x060007cb | 11e3d83aa704fa79d5d85c650b79c66d22a02c6d945185ac95cac169efa96493 | Compare row factor arrays via Eq_b(m_rowdim, m1.m_rowdim); ignore column geometry. | Same-artifact IL; 4 direct cases; state paths limited as documented in Lane B checkpoint | Verified |
| rc_dim..ctor(class:568[], class:568[]) | 0x060007b3 | 36f0aab7306cee9c64d72148666c18aa9084f1bfa0a74b4233e0a739d7294d9b | Initialize base rows from r.Length, cols from c.Length; assign m_rowdim to r, m_coldim to c. | Same-artifact IL; 6 direct cases; state paths limited as documented in Lane B checkpoint | Verified |
| rc_dim.make_rc(class:568[], class:568[]) | 0x060007b4 | 137790307c7b431b8b12f55af9aecd00aea49189f013cf73acb27b81411dbf0d | Construct and return new rc_dim instance with supplied row and column arrays. | Same-artifact IL; 6 direct cases; state paths limited as documented in Lane B checkpoint | Verified |
| rc_dim.eq_b(class:572) | 0x060007ba | f1a3e954a080eec13df74fa453d26474ca4751efae3bef0d366e12a979d788f5 | Dispatch equality to m2.eq2_b(this). | Same-artifact IL; 7 direct cases; state paths limited as documented in Lane B checkpoint | Verified |
| rc_dim.eq2_b(class:575) | 0x060007bb | e7178386af3591bf902bfdd49bd41cbeff74a23e0b7c727f2d9c576c6df97948 | Compare row and column factor arrays via Eq_b(m_rowdim, m1.m_rowdim) & Eq_b(m_coldim, m1.m_coldim). | Same-artifact IL; 6 direct cases; state paths limited as documented in Lane B checkpoint | Verified |
| crazy_dim..ctor(MatVal) | 0x060007ad | 7ff0599165db240939134df5fcff1eaede0e162d44cb06e96bde6851d83fb242 | Initialize base rows from m.Rows, cols from m.Cols; assign m_matrix to m. | Same-artifact IL; 6 direct cases; state paths limited as documented in Lane B checkpoint | Verified |
| crazy_dim.make(MatVal) | 0x060007b2 | de5613a5a2ce61aa5c74eb1ca33d5ee947b3cb2f30c50bfeb8932bd417d837aa | Construct and return new crazy_dim instance wrapping supplied matrix. | Same-artifact IL; 6 direct cases; state paths limited as documented in Lane B checkpoint | Verified |
| crazy_dim.eq_b(class:572) | 0x060007b0 | 1aaa594cfb249e74c879b2c072d8c63e6e4acb519b17138baf488e7fa772ac0b | Dispatch equality to m2.eq2_b(this). | Same-artifact IL; 9 direct cases; state paths limited as documented in Lane B checkpoint | Verified |
| crazy_dim.eq2_b(class:576) | 0x060007b1 | 6ac593fa6e76cfd5ea7a17da09eb21e7aadb3f985e7ae7447a870a6cecb7a5bf | Compare matrix shape then every cell dimension via Eq_b(matdim(r, c), m1.matdim(r, c)). | Same-artifact IL; 7 direct cases; state paths limited as documented in Lane B checkpoint | Verified |
| uniform_dim..ctor(class:568, i4, i4) | 0x0600079c | 23d7d3c3e14e5c000e5816e85d0e37477eadcd8ee9062fed68f18ac546cef38f | Initialize base rows/cols; assign m_dim to d if non-null, else Dim.identity. | Same-artifact IL; 5 direct cases; state paths limited as documented in Lane B checkpoint | Verified |
| uniform_dim.make(class:568, i4, i4) | 0x0600079d | 5b96c59812b7b1a9af0a76bea91986b311a8e4f7ca4b7c596dc01f4eb20aa73d | Construct and return new uniform_dim instance with supplied dimension and shape. | Same-artifact IL; 5 direct cases; state paths limited as documented in Lane B checkpoint | Verified |
| uniform_dim.dim(i4, i4) | 0x0600079e | 1f97f11a7ab9be5f5e8aea15ba57b295e96a344533ab1ac617f1e70358446cb1 | Return stored uniform dimension m_dim for any coordinates. | Same-artifact IL; 4 direct cases; state paths limited as documented in Lane B checkpoint | Verified |
| uniform_dim.matdim(i4, i4) | 0x0600079f | 49f2e9ff897276f1f0f699521c1bd6a4f9b8464f8ccade713be821923bed5465 | Return this instance identity for any coordinates. | Same-artifact IL; 4 direct cases; state paths limited as documented in Lane B checkpoint | Verified |
| uniform_dim.get_UniformDim() | 0x060007a0 | 4bb5be683bcb7f01a671eb3f290c4be4b08e97d898c1d272b910e0ce7ff850f6 | Return stored uniform dimension m_dim. | Same-artifact IL; 3 direct cases; state paths limited as documented in Lane B checkpoint | Verified |
| uniform_dim.get_IsLinear() | 0x060007a1 | 3f4a8904e79dcb4f754a3d797ac2940a64ce67abeda8a981547dd0b68a727349 | Return this instance identity. | Same-artifact IL; 2 direct cases; state paths limited as documented in Lane B checkpoint | Verified |
| uniform_dim.transpose() | 0x060007a9 | 48aeed5866f2184939bbc45373e58203ba17669f8e552ec00be9e6bdc749f8cf | Return this instance identity. | Same-artifact IL; 3 direct cases; state paths limited as documented in Lane B checkpoint | Verified |
| uniform_dim.eq_b(class:572) | 0x060007a2 | c1ab43140edd236a05dcf85a3883e24cf988f49903ee1b8ff97b568d54e6dbe5 | Dispatch equality to m2.eq2_b(m_dim). | Same-artifact IL; 7 direct cases; state paths limited as documented in Lane B checkpoint | Verified |
| uniform_dim.eq2_b(class:568) | 0x060007a3 | 1e6e753b6c063dcaf6acb620558a456be804d0a774385708691e19fbfe5067ec | Compare stored dimension via Dim.Eq_b(m_dim, d2); ignore matrix shapes. | Same-artifact IL; 5 direct cases; state paths limited as documented in Lane B checkpoint | Verified |

## Fields

| Owner / field | Token | Contract | Status |
|---|---|---|---|
| Mcd.MatDim.m_row | 0x04000128 | Storage/shape field; preserve original declaration and constructor assignment. | Tested |
| Mcd.MatDim.m_col | 0x04000129 | Storage/shape field; preserve original declaration and constructor assignment. | Tested |

## Test cases and open work

MatDimTests: 3/3 passed in process. MatDimDifferentialTests: 1/1 passed against the installed assembly in process (0 skipped), with six left arrays × six right arrays (empty, singleton, length/time, squared-length/velocity, polymorphic, and unequal lengths). Each pair compares Eq_b, IsScalarMultiple and Mult. Each left array also tests Uniform, Invert, scalar Mult, Product, and Push. Null Uniform/CMult and null-array Invert/scalar Mult/Product/Push are compared. Null-array helpers dereference their input and raise NullReferenceException rather than argument-validation errors. Base shape/null queries and coordinate extraction have independent grid assertions; no direct comparison of base members is claimed. No whole-type equivalence is claimed. Matrix pointer canonicalization, access/mutation, dimension factories, nested dimensions, arithmetic, copy-on-write, and native lifecycle remain outside this foundation contract.

## Remaining MatDim contracts (implementation in progress)

Same-artifact REA member inventory and ILSpy view establish the following additional contracts.
Per-member tokens/hashes and test statuses are appended when these members are validated.

- Base dispatch: Eq_b, MMult, DMult, Transpose and Det call the corresponding virtual member without null checks.
  Unhandled multiplication, dot product, inverse, transpose, determinant and power return null; unhandled
  equality returns false. Eq raises formatted mismatch errors only when equality fails. Pwr handles positive,
  negative and zero exponents through power/inverse dispatch; zero first checks whether squaring is possible.
- Uniform dimensions replace null with identity, preserve supplied shape, answer every coordinate with the same
  dimension, and return themselves for coordinate matrix dimensions and transpose. Equality ignores shape and
  uses Dim.Eq_b. Uniform multiplication combines units and left rows/right cols; dot multiplication returns the
  unit product. Inverse/power preserve shape; determinant raises the unit to the requested size.
- Row/column dimensions retain the supplied arrays by reference. Coordinate lookup indexes only the applicable
  axis; coordinate matrix dimensions are new 1×1 uniform dimensions. Row/column array access on the invariant
  axis returns the original array. Equality compares same-kind unit arrays and ignores the invariant axis size.
  Transpose swaps axis; inverse inverts unit arrays, including the original column inverse's output column count.
  Determinant multiplies the array dimensions. No row/column power override exists.
- Row-column dimensions retain both arrays; a coordinate multiplies its row and column dimensions. Equality
  compares both factors separately (even if alternate factors describe the same units). Transpose swaps arrays;
  inverse swaps and inverts factors; determinant multiplies both arrays. Power requires a constant product
  between column and row factors. Double-dispatch multiplication combines compatible inner-axis units and
  preserves the declared outer shapes; unsupported combinations return null.
- Crazy dimensions retain the matrix reference and query its element dimensions/matrix dimensions. Equality
  requires equal shapes and compares all element matrix dimensions. Other linear algebra operations use defaults.
- Factories detect uniform, then single-column, then single-row matrices, then row/column separability. A matrix
  that cannot be factored becomes crazy_dim; the linear-only array overload returns its IsLinear property.
  Empty/unreachable/invalid pointers retain the original helper errors. Uniform array factory uses dims.Length
  for BOTH axes, ignoring the supplied invariant count; this is intentional original behavior, not a correction.
  The two-factor factory first tries a uniform row factor, then a uniform column factor, then retains both arrays.
- Error messages use tab-separated key, fixed dimension text, and zero-based element indices. PickLoser scans
  columns==1 as vector, otherwise row then column; it reports the winner's first differing dimension, or raises
  array unit mismatch when no element difference is found. No shape prevalidation is added.
