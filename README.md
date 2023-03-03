# ImageQuantization
**The final project for the algorithms course is this one**
## Team Members:
|         Name  | GitHub |
| ------------- | ------------- |
| Hadi Ehab Ragaa  |[HodBossHod](https://github.com/HodBossHod)  |
| Hady Ahmed Abd-Elsalame  | [HadyAhmed00](https://github.com/HadyAhmed00)  |
| Hady Atef saeed  | [hady-o](https://github.com/hady-o)  |
## We have used a linked library containing the implementation of Fast Priority Queue in order to use it in our MST. 
For more information, refer to [here](https://github.com/BlueRaja/High-Speed-Priority-Queue-for-C-Sharp/wiki/Getting-Started)
<br>

## Architecture

  Edge Class 
  Extending from `FastPriorityQueueNode` Class<br>
  Attributes: -<br>
    `vert` integer <br>
    `parent` integer <br>
## Graph construction
  ### 	`getDistinctColors()` function:
  **Description:** <br>
  No parameters <br>
  a hash set of integers is defined named `distinctSet` which will only add the unique colors of the image. 
  Then, a nested for loop is created to pass on every single pixel in our input image `aimImage`. The outer loop passes on x-coordinates,
  while the inner loop passes on y-coordinates. Inside the inner loop, `codeColors` function is called taking the current pixel as an argument.
  The function returns an integer of the final encoded Color stored in `encodedColor` integer variable, which is added to `distinctSet` set in the following line.
  At the end of the function, a list named `listOfDistinct` is defined which will store the “distinctSet” after being converted to list then the count of the list is
  stored in `noColors` variable.<br>
  **Code:**
  <br>
  
  ```
public void getDistinctColors()
{

    HashSet<int> distinctSet = new HashSet<int>();
    for (int x = 0; x < aimImage.GetLength(0); x++)
    {
        for (int y = 0; y < aimImage.GetLength(1); y++)
        {
            int encodedColor = colorCoding.codeColors(aimImage[x, y]);
            distinctSet.Add(encodedColor);
        }
    }
    listOfDistinct = distinctSet.ToList();
    noColors = listOfDistinct.Count;
}
```
**Analysis:**

Function’s Order: $\ O (outer loop) *O (inner loop) *O (inner body) + O(rest of the function) $

Let width of image = N and height of image =H.

Final Order =  $\ O(N^2)$
<br>

### 	`getEculideanDistance() ` function:
  **Description:** <br>
  *parameters:* “src” & “dst” of type “Edge” Class which carries the vertex node and its parent node <br>
*body:* the nodes of `src` and `dst` of type `Edge` are decoded using `decodeColors()` function to use red,
blue and green separately and calculate the Eculidean distance between `src` and `dst` and return `res` variable storing this Ecuildean distance.
<br>
  **Code:**
  <br>
  
  ```
public static double getEculideanDistance(Edge src, Edge dst)
{
    colorCodingClass =new colorCodingClass();
    double res;
    RGBPixel srcRGB = colorCodingClass.decodeColors(src.vert);
    RGBPixel dstRGB = colorCodingClass.decodeColors(dst.vert);


    float X = dstRGB.red - srcRGB.red;
    float Y = dstRGB.green - srcRGB.green;
    float Z = dstRGB.blue - srcRGB.blue;
    res = Math.Sqrt((X * X) + (Y * Y) + (Z * Z));
    return res;
}

```
**Analysis:**

Final Order =  $\ O(1)$

### 	`codeColors()` function:
  **Description:** <br>
  *parameters:* “pixel” of type “RGBPixel”.  <br>
*body:* the whole color code of `pixel` is stored inside an integer variable named `enCodedColor`. This is done by adding 
the red color (represented in 1 byte) after being shifted to the left by 16 bits to the green color (represented in 1 byte) after being shifted to the left
by 8 bits. Then,the result is added to the blue color. Finally, `enCodedColor` variable, which holds the code of the three colors together, is returned at the end.
<br>
  **Code:**
  <br>
  
  ```
public int codeColors(RGBPixel pixel)
{

    int enCodedColor = (pixel.red << 16) + (pixel.green << 8) + pixel.blue;
    return enCodedColor;
}
```
**Analysis:**

Final Order =  $\ O(1)$

### 	`decodeColors() ` function:
  **Description:** <br>
  *parameters:* `codedColor` of type integer which carries the whole RGB code.   <br>
*body:* `res` variable of type “RGBPixel” carries three attributes of type `byte`; `red`, `green` 
and `blue`. We set red to “codedColor” after being shifted rightwards by 16 bits casted to byte.
The same applies to green but 8 bits not 16. Finally, blue is set to `codedColor` casted to byte and at 
the end `res` variable is returned.
<br>
  **Code:**
  <br>
  
  ```
public RGBPixel decodeColors(int codedColor)
{
  RGBPixel res;
  res.red = (byte)(codedColor >> 16);
  res.green = (byte)(codedColor >> 8);
  res.blue = (byte)(codedColor);
  return res;
}

```
**Analysis:**
Final Order =  $\ O(1)$
<br>

------------------------------------------------------------------------------------------------------------------

<br>

## Minimum Spanning Tree
`buildingMST()` function:
**Description:** <br>
  *parameters:* No parameters   <br>
*body:* A fast priority queue of type edge “fQueue” is defined with the same size as the list of
distinct colors size. In the first for loop, all the edges are enqueued inside “fQueue”. Each edge
takes the current vertex of “listOfDistinct” with the vertex’s parent set to -1 and the weight is
initialized to infinity. In the next line, we defined a variable of type float named “tmp”. Then,
we relax all the edges using a while loop. The loop checks that the queue is not empty to start iterating.
Inside the loop, “front” of type edge takes the dequeued queue’s front. The while loop mentioned above
contains an if condition and a foreach loop. The if condition checks that the front is not the root.
If so the “totalWeight” is increased by the value of the front priority and the front edge is added to 
“minSpanningTreeEdges” list of edges. Considering the foreach loop, it passes on each edge inside the
“fQueue” except the dequeued front. Then, “tmp” variable carries the value that is returned from 
“getEculideanDistance” function calculating the distance between the current edge “e” and the front edge.
If this “tmp” is smaller than the priority of current edge “e”, the parent of “e” is updated to the “vert” 
of front and the priority of “fQueue” is updated again to rearrange the queue.
<br>
  **Code:**
  <br>
  
  ```
  private void buildingMST()
        {
            FastPriorityQueue<Edge> fQueue = new FastPriorityQueue<Edge>(listOfDistinct.Count);
            //All the edges are enqueued inside fQueue
            //Each edge takes the current vertex of "listOfDistinct" with parent set to -1
            //Edge's weight is set to infinity
            for (int i = 0; i < listOfDistinct.Count; i++)
                fQueue.Enqueue(new Edge(listOfDistinct[i], -1), int.MaxValue);
            float tmp;
            while (fQueue.Count != 0) //if the queue isn't empty
            {
                Edge front = fQueue.Dequeue(); //the queue's front is dequeued
                if (front.parent != -1)  //if the front is not the root
                {
                    totalWeight+= front.Priority;
                    minSpanningTreeEdges.Add(front);
                }
                foreach (var e in fQueue) //each vertex inside the queue except front
                {
                    tmp = (float)getDistanceClass.getEculideanDistance(e, front);
                    if (tmp < e.Priority) 
                    {
                        e.parent=(front.vert); //setting the parent of e to front’s vert
                        fQueue.UpdatePriority(e, tmp); //Updating queue
                    }
                }
            }
        }
```
  **Analysis**
  Final Order =  $\ O(D * E log(V))$
  
  
  
  
  
  






