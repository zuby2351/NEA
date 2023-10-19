using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Save : MonoBehaviour
{
    // Start is called before the first frame update
    public void SaveAsFile()
    {
        int numberOfNodes = GetNumberOfNodes();
        Vector3[] nodePositions = new Vector3[numberOfNodes];
        for (int i = 0; i < numberOfNodes; i++)
        {
            Transform node = GetNodeTransform(i); // Replace with your method to get node Transform
            if (node != null)
            {
                nodePositions[i] = node.position;
            }
        }
        SaveTrackData(numberOfNodes, nodePositions);
    }

    public void SaveTrackData(int numberOfNodes, Vector3[] nodePositions)
    {
        // Define the path where you want to save the file
        string filePath = Path.Combine(Application.dataPath, "save1.txt");

        // Create or overwrite the file
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            // Write the number of nodes as the first line
            writer.WriteLine(numberOfNodes);

            // Write node positions
            for (int i = 0; i < numberOfNodes; i++)
            {
                // Write each node's position as a new line
                writer.WriteLine(nodePositions[i].ToString());
            }
        }

        // Optionally, print a message to the console to indicate that the data has been saved
        Debug.Log("Track data saved to " + filePath);
    }

    public Transform GetNodeTransform(int index)
    {
        return GameObject.Find((index + 1).ToString()).transform;
    }

    public int GetNumberOfNodes()
    {
        int numberOfNodes = 0;
        while (true)
        {
            string nodeName = (numberOfNodes+1).ToString(); // Construct the expected node name
            GameObject nodeObject = GameObject.Find(nodeName);

            // If we found a GameObject with the expected name, increment the count.
            if (nodeObject != null)
            {
                numberOfNodes++;
            }
            else
            {
                // If no GameObject was found with the expected name, break the loop.
                break;
            }
        }

        return numberOfNodes;
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
