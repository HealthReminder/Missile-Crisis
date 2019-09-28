using System.Collections.Generic;
using System;
public static class ArrayConcatenation
{
    public static byte[][] UnmergeArrays(byte[] array) {
        List<byte[]> found_arrays = new List<byte[]>();
        bool is_length = true;
        int array_length = -999;
        for (int i = 0; i < array.Length;)
        {
            if(is_length) {
                array_length = Convert.ToInt32(array[i]);
                //Debug.Log("Found length of "+array_length+" in "+i);
                i+=1;
            } else {
                byte[] read_array = new byte[array_length];
                System.Array.Copy(array,i,read_array,0,array_length);
                //Debug.Log("Copied "+read_array.Length+" bytes from "+i+" ending on"+ (i+array_length));
                found_arrays.Add(read_array);
                i+=array_length;
            }
            is_length = !is_length;
        }
        byte[][] results_array = new byte[found_arrays.Count][];
        for (int i = 0; i < found_arrays.Count; i++)
            results_array[i] = found_arrays[i];
        
        return(results_array);
    }
    public static byte[] MergeArrays(byte[][] arrays) {
        //Calculate how many bytes the result array will need
        byte[] result_array= new byte[CalculateLength(arrays)];
        //Put everything together
        //Alternating between the size of the array and its content
        bool is_length = true;
        int current_array = 0;
        for (int i = 0; i < result_array.Length;)
        {
            if(is_length) {
                byte lenght_byte = Convert.ToByte(arrays[current_array].Length);
                result_array[i] = lenght_byte;                
                //Debug.Log("Copied "+lenght_byte+" lenght to "+i);
                i+=1;
            } else {
                arrays[current_array].CopyTo(result_array,i);
                //Debug.Log("Copied "+arrays[current_array].Length+" bytes to "+i+" ending on"+ i+arrays[current_array].Length);
                i+= arrays[current_array].Length;
                current_array+=1;
            }
            is_length = !is_length;
        }
        return(result_array);
    }
    static int CalculateLength(byte[][] arrays) {
        int byte_quantity = 0;
        foreach (Byte[] b in arrays)
            byte_quantity += b.Length+1;
        return(byte_quantity);
    }
}
