//#define Trace

// ParallelDeflateOutputStream.cs
// ------------------------------------------------------------------
//
// A DeflateStream that does compression only, it uses a
// divide-and-conquer approach with multiple threads to exploit multiple
// CPUs for the DEFLATE computation.
//
// last saved:
// Time-stamp: <2010-January-20 19:24:58>
// ------------------------------------------------------------------
//
// Copyright (c) 2009-2010 by Dino Chiesa
// All rights reserved!
//
// ------------------------------------------------------------------

