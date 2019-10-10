using UnityEngine;
using System.Collections;

public class ScreenShakeBehaviour : MonoBehaviour {
    [SerializeField]
    Vector3 maximumTranslationShake = Vector3.one;

    [SerializeField]
    Vector3 maximumAngularShake = Vector3.one * 15;

    [SerializeField]
    float frequency = 25;
    [SerializeField]
    float traumaExponent = 1;
    [SerializeField]
    float recoverySpeed = 1;
    private float trauma;

    private float seed;

    private void Awake()
    {
        seed = Random.value;
    }

    private void Update()
    {

        if(Input.GetKeyDown(KeyCode.V)) {
            InduceStress(1);
        } else if(Input.GetKeyDown(KeyCode.B)) {
            InduceStress(10);
        }

        // Taking trauma to an exponent allows the ability to smoothen
        // out the transition from shaking to being static.
        float shake = Mathf.Pow(trauma, traumaExponent);

        // This x value of each Perlin noise sample is fixed,
        // allowing a vertical strip of noise to be sampled by each dimension
        // of the translational and rotational shake.
        // PerlinNoise returns a value in the 0...1 range; this is transformed to
        // be in the -1...1 range to ensure the shake travels in all directions.
        transform.localPosition = new Vector3(
            maximumTranslationShake.x * (Mathf.PerlinNoise(seed, Time.time * frequency) * 2 - 1),
            maximumTranslationShake.y * (Mathf.PerlinNoise(seed + 1, Time.time * frequency) * 2 - 1),
            maximumTranslationShake.z * (Mathf.PerlinNoise(seed + 2, Time.time * frequency) * 2 - 1)
        ) * shake;

        transform.localRotation = Quaternion.Euler(new Vector3(
            maximumAngularShake.x * (Mathf.PerlinNoise(seed + 3, Time.time * frequency) * 2 - 1),
            maximumAngularShake.y * (Mathf.PerlinNoise(seed + 4, Time.time * frequency) * 2 - 1),
            maximumAngularShake.z * (Mathf.PerlinNoise(seed + 5, Time.time * frequency) * 2 - 1)
        ) * shake);

        trauma = Mathf.Clamp01(trauma - recoverySpeed * Time.deltaTime);
    }

    public void InduceStress(float stress)
    {
        trauma = Mathf.Clamp01(trauma + stress);
    }
}