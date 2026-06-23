import requests
import csv

def download_snomed_specialties():
    # SNOMED Code for "Clinical specialty (qualifier value)" is 394658006
    # The ECL query <<394658006 asks for all descendants of Clinical Specialty
    # URL encoded << is %3C%3C
    url = "https://r4.ontoserver.csiro.au/fhir/ValueSet/$expand?url=http://snomed.info/sct?fhir_vs=ecl/%3C%3C394658006&count=5000"

    headers = {
        "Accept": "application/fhir+json"
    }

    print("Connecting to public FHIR server (Ontoserver)...")
    print("Downloading FHIR Clinical Specialty dataset...")

    try:
        response = requests.get(url, headers=headers, timeout=60)
        response.raise_for_status() 
        
        data = response.json()
        
        if 'expansion' in data and 'contains' in data['expansion']:
            items = data['expansion']['contains']
            
            unique_codes = set()
            specialties_list = []
            
            print("Processing data and removing duplicates...")
            
            for item in items:
                code = item.get('code')
                display = item.get('display')
                
                if code and display and code not in unique_codes:
                    unique_codes.add(code)
                    specialties_list.append([code, display, "SNOMED"])
            
            output_filename = "snomed_specialties_dataset.csv"
            
            with open(output_filename, mode='w', newline='', encoding='utf-8') as file:
                writer = csv.writer(file)
                writer.writerow(["Code", "DisplayName", "CodeSystem"]) 
                writer.writerows(specialties_list)
                
            print(f"SUCCESS! Extracted {len(specialties_list)} unique medical specialties.")
            print(f"Saved locally to: {output_filename}")
            print("You can now use this CSV file to seed your C# Database!")
            
        else:
            print("Error: No expansion data was found.")

    except requests.exceptions.Timeout:
        print("Error: The request timed out. The FHIR server is taking too long.")
    except requests.exceptions.RequestException as e:
        print(f"Error: Failed to connect to the FHIR server. Details: {e}")

if __name__ == "__main__":
    download_snomed_specialties()