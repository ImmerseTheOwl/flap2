![Alt text](Slide1.jpeg)

## Inspiration

We live, think, and remember in a 3D world—so why are we still using 2D surfaces to store our thoughts? Inspired by the method of loci—an ancient mnemonic technique where memories are tied to specific physical locations—we harness the human brain’s innate spatial memory. By turning notetaking into an immersive, spatio-temporal activity, we activate the brain’s parietal cortex to organize and recall information naturally and intuitively. This prototype bridges cognition and technology, unlocking the full potential of human memory and creativity.


## What it does

Picture this: learning Spanish vocabulary becomes intuitive—el sofá belongs in your virtual living room, while el libro rests on a shelf. FLAP is a tool that transforms how you organize and remember information by leveraging the power of spatial memory. Everyday tasks? Imagine a to-do list where household chores are anchored in the laundry room, groceries in the kitchen, and work projects on your desk—instantly reducing cognitive load and boosting productivity. Need to prepare for a speech? Place your key points in spatial locations: the introduction by the entrance, core arguments in the center, and the conclusion near the exit. By walking through your ideas, you naturally strengthen understanding and recall. 

Like butterflies settling in a garden, ideas are put in space. Instead of forcing our naturally spatial thinking into flat diagrams, Flap lets our ideas exist where they make the most sense – in the rich, three-dimensional world we actually live in.


## How we built it
* Designed and modeled our custom butterfly notes in Blender, where each animated butterfly carries a sticky note
* Built upon Meta XR All-in-One SDK's building blocks, using poke interaction, grab interaction and scene scanning to enable butterflies to perch in designated locations
* Optimized for Meta Quest 3's capabilities, using passthrough and precise hand tracking for seamless AR experience
* Utilized Meta’s Voice SDK to enable dictation when creating notes

## Challenges we ran into
We ran into significant challenges in hardware, as it always happens in hackathons. It was also challenging to incorporate parts of the codebase that were independently worked on by different members of the team. With varied hackathon experience, it was hard to settle on what we could realistically accomplish in the time we had. We also ran into difficulties with the building blocks tool, which was new and didn’t have many tutorials available. 

## Accomplishments that we're proud of
We’re proud to have developed a product grounded in cognitive and HCI research—a foundation we believe is key to creating tools and experiences that genuinely enhance human life. We’re excited to take this work further by conducting research to explore how spatial interactions can improve memory retention.

As a team of hackers from diverse backgrounds and newcomers to the world of VR, we’re particularly proud of immersing ourselves in Unity’s development environment. By leveraging tools like Meta’s Building Blocks, we built a deeper understanding of the rich set of interactions available to VR developers.


## What we learned
We learned to reverse-engineer sample scenes to understand what was happening under the hood when documentation was lacking. By poking around, we gained a deeper understanding of the Meta developer ecosystem in unity and became more confident in navigating it.

## What's next for FLAP
As a team of HCI researchers, software developers, and educators, we are dedicated to advancing FLAP’s potential. Our immediate focus is on conducting experiments to validate the concept of using spatial interactions to enhance memory and reduce cognitive load. Through this research, we aim to gain deeper insights into how these interactions, enabled by technological innovation, can unlock human cognitive potential.

## Repository
The main branch contains a working version where the butterflies automatically flap into their appropriate place, categorized by machine learning. Within it, the `DEMO` folder contains the working version of the demo shown in the video, which allows users to place butterfly sticky-notes to space. 