import requests
import csv

def download_snomed_chronic_diseases():
    # SNOMED Code for "Chronic disease (disorder)" is 27624003
    # The ECL query <<27624003 asks for all descendants of Chronic Disease
    # URL encoded << is %3C%3C
    url = "https://r4.ontoserver.csiro.au/fhir/ValueSet/$expand?url=http://snomed.info/sct?fhir_vs=ecl/%3C%3C27624003&count=15000"

    headers = {
        "Accept": "application/fhir+json"
    }

    print("Connecting to public FHIR server (Ontoserver)...")
    print("Downloading massive Chronic Disease dataset. This may take a minute...")

    try:
        response = requests.get(url, headers=headers, timeout=120)
        response.raise_for_status() 
        
        data = response.json()
        
        if 'expansion' in data and 'contains' in data['expansion']:
            items = data['expansion']['contains']
            
            unique_codes = set()
            diseases_list = []
            
            print("Processing data and removing duplicates...")
            
            for item in items:
                code = item.get('code')
                display = item.get('display')
                
                if code and display and code not in unique_codes:
                    unique_codes.add(code)
                    diseases_list.append([code, display, "SNOMED"])
            
            output_filename = "snomed_chronic_diseases_dataset.csv"
            
            with open(output_filename, mode='w', newline='', encoding='utf-8') as file:
                writer = csv.writer(file)
                writer.writerow(["Code", "DisplayName", "CodeSystem"]) 
                writer.writerows(diseases_list)
                
            print(f"SUCCESS! Extracted {len(diseases_list)} unique chronic diseases.")
            print(f"Saved locally to: {output_filename}")
            print("You can now use this CSV file to seed your C# Database!")
            
        else:
            print("Error: No expansion data was found.")

    except requests.exceptions.Timeout:
        print("Error: The request timed out. The FHIR server is taking too long.")
    except requests.exceptions.RequestException as e:
        print(f"Error: Failed to connect to the FHIR server. Details: {e}")

if __name__ == "__main__":
    download_snomed_chronic_diseases()