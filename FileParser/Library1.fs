module Parser

open System
open System.IO

let parseLM(path : string, signal : byte) = 
    let buf = File.ReadAllBytes(path)
    let mutable topF = 0L

    let answ = [| for i in 0..4..buf.Length-1 do
                    if buf.[i+3] = 244uy then topF <- topF + 1L                    
                    if buf.[i+3] = signal then                                     
                        let b1 = int64 buf.[i]                                        
                        let b2 = int64 buf.[i+1]                                        
                        let b3 = int64 buf.[i+2]
                        let res = (b1 + (b2 <<< 8) + (b3 <<< 16) + (topF <<< 24))
                        yield res|]                                                                                                                                                                    
    answ