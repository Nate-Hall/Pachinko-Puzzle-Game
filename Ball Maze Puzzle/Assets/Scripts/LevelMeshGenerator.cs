using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LevelMeshGenerator {

	const float obstacleFrontZ = -0.15f;
	const float obstacleBackZ = 0;

	const int OBSTACLE_VALUE = 2;



	public static List<Transform> GenerateMeshes(int[,] gridValues) {
		List<Transform> obstacles = new List<Transform>();

		for (int x = 0; x < gridValues.GetLength(0); x++) {
			for (int y = gridValues.GetLength(1) - 1; y >= 0; y--) {
				if (gridValues[x, y] == OBSTACLE_VALUE) {
					Vector2[] obstaclePoints = GenerateObstaclePoints(gridValues, x, y);
					obstacles.Add(CreateObstacleObject(obstaclePoints));
				}
			}
		}

		return obstacles;
	}



	static Vector2[] GenerateObstaclePoints(int[,] gridValues, int startX, int startY) {

		int[,] obstacleValues = gridValues;
		Queue<Vector2> obstacleCoords = new Queue<Vector2>();
		int currentX = startX;
		int currentTopY = startY;
		int currentBottomY = startY + 1;
		int currentVertexSide = -1;

		// First Column check //

		if (CheckPixelInDirectionForObstacle(obstacleValues, startX + 1, startY + 1)) {
			obstacleCoords.Enqueue(AddVertexOfPixelToQueue(obstacleValues, startX, startY, -1, -1));
		} else {
			obstacleCoords.Enqueue(AddVertexOfPixelToQueue(obstacleValues, startX, startY, -1, 1));
		}

		while (currentBottomY > 0) {
			if (CheckPixelInDirectionForObstacle(obstacleValues, startX, currentBottomY) && !CheckPixelInDirectionForObstacle(obstacleValues, startX, currentBottomY - 1)) {
				break;
			} else {
				currentBottomY--;
			}
		}
		obstacleCoords.Enqueue(AddVertexOfPixelToQueue(obstacleValues, startX, currentBottomY, -1, -1));

		if (!CheckPixelInDirectionForObstacle(obstacleValues, startX + 1, currentTopY) && CheckPixelInDirectionForObstacle(obstacleValues, startX + 1, currentTopY - 1)) {
			obstacleCoords.Enqueue(AddVertexOfPixelToQueue(obstacleValues, startX, startY, 1, -1));
		} else {
			obstacleCoords.Enqueue(AddVertexOfPixelToQueue(obstacleValues, startX, startY, 1, 1));
		}

		if (CheckPixelInDirectionForObstacle(obstacleValues, startX + 1, currentBottomY - 1)) {
			obstacleCoords.Enqueue(AddVertexOfPixelToQueue(obstacleValues, startX, currentBottomY - 1, 1, -1));
		} else {
			obstacleCoords.Enqueue(AddVertexOfPixelToQueue(obstacleValues, startX, currentBottomY, 1, -1));
		}
		currentVertexSide = 1;

		for (int i = currentTopY; i >= currentBottomY; i--) {
			obstacleValues[startX, i] = 0;
		}

		currentX++;

		// Other Columns //

		bool endOfObstacle = false;

		while (!endOfObstacle) {
			endOfObstacle = true;
			int previousTopY = currentTopY;
			currentTopY = currentBottomY - 1;

			for (int y = currentTopY; y < obstacleValues.GetLength(1); y++) {
				if (CheckPixelInDirectionForObstacle(obstacleValues, currentX, y) && (y == obstacleValues.GetLength(1) - 1 || !CheckPixelInDirectionForObstacle(obstacleValues, currentX, y + 1))) {
					if (!CheckPixelInDirectionForObstacle(obstacleValues, currentX + 1, y) && CheckPixelInDirectionForObstacle(obstacleValues, currentX + 1, y - 1)) {
						obstacleCoords.Enqueue(AddVertexOfPixelToQueue(obstacleValues, currentX, y, currentVertexSide, -1));
					} else if (previousTopY > y && !CheckPixelInDirectionForObstacle(obstacleValues, currentX + 1, y + 1) && !CheckPixelInDirectionForObstacle(obstacleValues, currentX + 1, y) && !CheckPixelInDirectionForObstacle(obstacleValues, currentX + 1, y - 1)) {
						obstacleCoords.Enqueue(AddVertexOfPixelToQueue(obstacleValues, currentX, y, currentVertexSide, -1));
					} else {
						obstacleCoords.Enqueue(AddVertexOfPixelToQueue(obstacleValues, currentX, y, currentVertexSide, 1));
					}
					currentTopY = y;
					previousTopY = currentTopY;
					currentBottomY = currentTopY;
					endOfObstacle = false;
					for (y = currentBottomY; y >= 0; y--) {
						if (CheckPixelInDirectionForObstacle(obstacleValues, currentX, y) && (y == 0 || !CheckPixelInDirectionForObstacle(obstacleValues, currentX, y - 1))) {
							if (CheckPixelInDirectionForObstacle(obstacleValues, currentX + 1, y - 1)) {
								obstacleCoords.Enqueue(AddVertexOfPixelToQueue(obstacleValues, currentX, y - 1, currentVertexSide, -1));
								currentBottomY = y - 1;
							} else {
								obstacleCoords.Enqueue(AddVertexOfPixelToQueue(obstacleValues, currentX, y, currentVertexSide, -1));
								currentBottomY = y;
							}

							for (int i = currentTopY; i >= currentBottomY; i--) {
								obstacleValues[currentX, i] = 0;
							}

							break;
						}
					}
					break;
				} else if (!CheckPixelInDirectionForObstacle(obstacleValues, currentX, y) && !CheckPixelInDirectionForObstacle(obstacleValues, currentX, y + 1) && !CheckPixelInDirectionForObstacle(obstacleValues, currentX, y + 2)) {
					break;
				}
			}

			currentX++;
		}

		return obstacleCoords.ToArray();
	}



	static bool CheckPixelInDirectionForObstacle(int[,] gridValues, int currentX, int currentY) {
		if (currentX < gridValues.GetLength(0) && currentX >= 0 && currentY < gridValues.GetLength(1) && currentY >= 0) {
			if (gridValues[currentX, currentY] == OBSTACLE_VALUE) {
				return true;
			}
		}
		return false;
	}



	static Vector2 AddVertexOfPixelToQueue(int[,] gridValues, int pixelX, int pixelY, int directionX, int directionY) {
		Vector2 originMin = new Vector2(-0.5f, -0.5f);
		Vector2 originMax = new Vector2(0.5f, 0.5f);
		Vector2 originSize = new Vector2(1, 1);
		Vector2 halfPixelSize = new Vector2(0.5f / (float)gridValues.GetLength(0), 0.5f / (float)gridValues.GetLength(1));

		return new Vector2(originMin.x + halfPixelSize.x + (halfPixelSize.x * pixelX * 2) + (directionX * halfPixelSize.x), originMin.y + halfPixelSize.y + (halfPixelSize.y * pixelY * 2) + (directionY * halfPixelSize.y));
	}


	static Transform CreateObstacleObject(Vector2[] obstacleCoords) {
		GameObject obstacle = new GameObject("Obstacle");
		MeshFilter meshFilter = obstacle.AddComponent<MeshFilter>();
		obstacle.AddComponent<MeshRenderer>();
		MeshCollider collider = obstacle.AddComponent<MeshCollider>();
		//Add once called from outside class
		//Put in parameter as well
		//obstacle.GetComponent<Renderer>().sharedMaterial = obstacleMaterial;

		Mesh mesh = meshFilter.mesh;
		collider.sharedMesh = mesh;

		Vector3[] newVertices;
		Vector2[] newUV;
		int[] newTriangles;

		newVertices = GenerateVertices(obstacleCoords);
		newTriangles = GenerateTriangles(obstacleCoords);
		newUV = GenerateUVs(obstacleCoords);

		mesh.Clear();

		mesh.vertices = newVertices;
		mesh.triangles = newTriangles;
		mesh.uv = newUV;
		mesh.RecalculateNormals();

		return obstacle.transform;
	}



	static Vector3[] GenerateVertices(Vector2[] points) {
		Vector3[] vertices = new Vector3[(points.Length * 6) - 4]; 

		int verticesIndex = 0;

		//Front face//

		for (int i = 0; i < points.Length; i++) {
			vertices[verticesIndex] = new Vector3(points[i].x, points[i].y, obstacleFrontZ);

			verticesIndex++;
		}

		//Middle faces//

		for (int i = 0; i < points.Length - 3; i+=2) {

			vertices[verticesIndex] = new Vector3(points[i].x, points[i].y, obstacleFrontZ);
			vertices[verticesIndex + 1] = new Vector3(points[i].x, points[i].y, obstacleBackZ);
			vertices[verticesIndex + 2] = new Vector3(points[i + 2].x, points[i + 2].y, obstacleBackZ);
			vertices[verticesIndex + 3] = new Vector3(points[i + 2].x, points[i + 2].y, obstacleFrontZ);
			verticesIndex += 4;
		}

		vertices[verticesIndex] = new Vector3(points[points.Length - 2].x, points[points.Length - 2].y, obstacleFrontZ);
		vertices[verticesIndex + 1] = new Vector3(points[points.Length - 2].x, points[points.Length - 2].y, obstacleBackZ);
		vertices[verticesIndex + 2] = new Vector3(points[points.Length - 1].x, points[points.Length - 1].y, obstacleBackZ);
		vertices[verticesIndex + 3] = new Vector3(points[points.Length - 1].x, points[points.Length - 1].y, obstacleFrontZ);
		verticesIndex += 4;

		for (int i = points.Length - 1; i > 2; i -= 2) {

			vertices[verticesIndex] = new Vector3(points[i].x, points[i].y, obstacleFrontZ);
			vertices[verticesIndex + 1] = new Vector3(points[i].x, points[i].y, obstacleBackZ);
			vertices[verticesIndex + 2] = new Vector3(points[i - 2].x, points[i - 2].y, obstacleBackZ);
			vertices[verticesIndex + 3] = new Vector3(points[i - 2].x, points[i - 2].y, obstacleFrontZ);
			verticesIndex += 4;
		}

		vertices[verticesIndex] = new Vector3(points[1].x, points[1].y, obstacleFrontZ);
		vertices[verticesIndex + 1] = new Vector3(points[1].x, points[1].y, obstacleBackZ);
		vertices[verticesIndex + 2] = new Vector3(points[0].x, points[0].y, obstacleBackZ);
		vertices[verticesIndex + 3] = new Vector3(points[0].x, points[0].y, obstacleFrontZ);

		return vertices;
	}



	static int[] GenerateTriangles(Vector2[] points) {
		int[] triangles = new int[(((points.Length / 2) - 1) * 6) + (points.Length * 6)];

		int currentVertex = 0;

		int vertexIndex = 0;

		//Front face//

		for (int i = 0; i < (((points.Length - 4) * 3) + 6); i+=6) {
			triangles[i] = currentVertex;
			triangles[i+1] = currentVertex+2;
			triangles[i+2] = currentVertex+3;

			triangles[i+3] = currentVertex+3;
			triangles[i+4] = currentVertex+1;
			triangles[i+5] = currentVertex;

			currentVertex += 2;
			vertexIndex += 6;
		}

		currentVertex += 2;

		//Side faces//

		for (int i = 0; i < points.Length; i++) {
			for (int j = 0; j < 3; j++) {
				triangles[vertexIndex + j] = currentVertex;
				currentVertex++;
			}
			vertexIndex += 3;

			triangles[vertexIndex] = currentVertex - 1;
			triangles[vertexIndex + 1] = currentVertex;
			triangles[vertexIndex + 2] = currentVertex - 3;
			currentVertex++;
			vertexIndex += 3;
		}

		return triangles;
	}



	static Vector2[] MorphObstacleToConvexShape(Vector2[] obstacleCoords) {
		Vector2[] oldPoints = obstacleCoords;
		Queue<Vector2> newPoints = new Queue<Vector2>();

		Vector2 previousPoint = new Vector2();
		Vector2 nextPoint = new Vector2();

		for (int i = 0; i < oldPoints.Length; i++) {
			if(i != oldPoints.Length - 1) {
				nextPoint = oldPoints[i + 1];
			} else {
				nextPoint = oldPoints[0];
			}

			if(previousPoint != null) {
				if (Vector3.Cross((oldPoints[i] - previousPoint).normalized, (nextPoint - oldPoints[i]).normalized).z < 0) {
					//Is right turn or straight
					newPoints.Enqueue(oldPoints[i]);
					previousPoint = oldPoints[i];
				}
			} else {
				newPoints.Enqueue(oldPoints[i]);
				previousPoint = oldPoints[i];
			}
		}

		return newPoints.ToArray();
	}



	static Vector2[] GenerateUVs(Vector2[] points) {
		Vector2[] uvs = new Vector2[(points.Length * 6) - 4];

		int verticesIndex = 0;

		float minX = points[0].x;
		float maxX = points[0].x;
		float minY = points[0].y;
		float maxY = points[0].y;

		for (int i = 0; i < points.Length; i++) {
			if(points[i].x < minX) {
				minX = points[i].x;
			} else if (points[i].x > maxX) {
				maxX = points[i].x;
			}

			if (points[i].y < minY) {
				minY = points[i].y;
			} else if (points[i].y > maxY) {
				maxY = points[i].y;
			}
		}

		float objWidth = maxX - minX;
		float objHeight = maxY - minY;

		float zDistance = obstacleFrontZ - obstacleBackZ;

		//Front face//

		for (int i = 0; i < points.Length; i++) {
			if (objWidth * 2 >= objHeight) {
				uvs[verticesIndex] = new Vector2((points[i].x - minX) / objWidth, (points[i].y - minY));
			} else {
				uvs[verticesIndex] = new Vector2((points[i].y - minY) / objHeight, (points[i].x - minX));
			}

			verticesIndex++;
		}

		//Middle faces//

		for (int i = 0; i < points.Length - 3; i += 2) {

			uvs[verticesIndex] = new Vector2((points[i].x - minX)/objWidth, 0);
			uvs[verticesIndex + 1] = new Vector2((points[i].x - minX) / objWidth, zDistance);
			uvs[verticesIndex + 2] = new Vector2((points[i + 2].x - minX) / objWidth, zDistance);
			uvs[verticesIndex + 3] = new Vector2((points[i + 2].x - minX) / objWidth, 0);
			verticesIndex += 4;
		}

		uvs[verticesIndex] = new Vector2((points[points.Length - 2].x - minX) / objWidth, 0);
		uvs[verticesIndex + 1] = new Vector2((points[points.Length - 2].x - minX) / objWidth, zDistance);
		uvs[verticesIndex + 2] = new Vector2((points[points.Length - 1].x - minX) / objWidth, zDistance);
		uvs[verticesIndex + 3] = new Vector2((points[points.Length - 1].x - minX) / objWidth, 0);
		verticesIndex += 4;

		for (int i = points.Length - 1; i > 2; i -= 2) {

			uvs[verticesIndex] = new Vector2((points[i].x - minX) / objWidth, 0);
			uvs[verticesIndex + 1] = new Vector2((points[i].x - minX) / objWidth, zDistance);
			uvs[verticesIndex + 2] = new Vector2((points[i - 2].x - minX) / objWidth, zDistance);
			uvs[verticesIndex + 3] = new Vector2((points[i - 2].x - minX) / objWidth, 0);
			verticesIndex += 4;
		}

		uvs[verticesIndex] = new Vector2((points[1].x - minX) / objWidth, 0);
		uvs[verticesIndex + 1] = new Vector2((points[1].x - minX) / objWidth, zDistance);
		uvs[verticesIndex + 2] = new Vector2((points[0].x - minX) / objWidth, zDistance);
		uvs[verticesIndex + 3] = new Vector2((points[0].x - minX) / objWidth, 0);

		return uvs;
	}
}
