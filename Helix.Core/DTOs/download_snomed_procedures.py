import requests
import csv
import time

def download_snomed_procedures():
    # Base URL without the count parameter
    base_url = "https://r4.ontoserver.csiro.au/fhir/ValueSet/$expand?url=http://snomed.info/sct?fhir_vs=ecl/%3C%3C71388002"

    headers = {
        "Accept": "application/fhir+json"
    }

    unique_codes = set()
    procedures_list = []
    
    # Pagination configuration
    count = 10000  # Safe chunk size
    offset = 0     # Starting point
    has_more_records = True

    print("Connecting to public FHIR server (Ontoserver)...")
    print("Downloading massive SNOMED Procedure dataset using pagination...")

    try:
        while has_more_records:
            # Append count and offset dynamically
            paginated_url = f"{base_url}&count={count}&offset={offset}"
            print(f"Fetching records {offset} to {offset + count}...")

            response = requests.get(paginated_url, headers=headers, timeout=120)
            response.raise_for_status() 
            
            data = response.json()
            
            if 'expansion' in data and 'contains' in data['expansion']:
                items = data['expansion']['contains']
                
                # If the server returns an empty list, we've reached the absolute end
                if not items:
                    has_more_records = False
                    break
                
                print(f" -> Processing {len(items)} items...")
                
                for item in items:
                    code = item.get('code')
                    display = item.get('display')
                    
                    if code and display and code not in unique_codes:
                        unique_codes.add(code)
                        procedures_list.append([code, display, "SNOMED"])
                
                # If we got back exactly as many as we asked for, there might be more
                if len(items) == count:
                    offset += count
                    # Sleep for 1 second to avoid rate-limiting/banning from the public server
                    time.sleep(1) 
                else:
                    # If we got fewer than 10,000, we've reached the end of the dataset!
                    has_more_records = False
            else:
                has_more_records = False
                print("No more expansion data found.")

        # Save everything to CSV
        output_filename = "snomed_procedures_dataset.csv"
        
        with open(output_filename, mode='w', newline='', encoding='utf-8') as file:
            writer = csv.writer(file)
            writer.writerow(["Code", "DisplayName", "CodeSystem"]) 
            writer.writerows(procedures_list)
            
        print(f"\nSUCCESS! Extracted a total of {len(procedures_list)} unique procedures.")
        print(f"Saved locally to: {output_filename}")

    except requests.exceptions.Timeout:
        print("Error: The request timed out. The FHIR server is taking too long.")
    except requests.exceptions.RequestException as e:
        print(f"Error: Failed to connect to the FHIR server. Details: {e}")

if __name__ == "__main__":
    download_snomed_procedures()